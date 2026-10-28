using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TrashPrefabIconCapturingTool : EditorWindow
{
    private DefaultAsset prefabFolder;
    private Camera captureCamera;

    private string outputFolder = "Assets/Project/Images";
    private int imageSize = 512;
    private float padding = 1.15f;
    private float rotationY = 25f;
    private float rotationX = -10f;

    private bool isCapturing;

    [MenuItem("Tools/Trash Prefab Capture")]
    private static void OpenWindow()
    {
        GetWindow<TrashPrefabIconCapturingTool>("Trash Capture");
    }

    private void OnGUI()
    {
        // how the gui for tool will look in editor tool.
        GUILayout.Label("Trash Prefab Capture", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox(
            "Finds prefabs in a folder and captures each as a PNG.",
            MessageType.Info
        );

        prefabFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Prefab Folder",
            prefabFolder,
            typeof(DefaultAsset),
            false
        );

        captureCamera = (Camera)EditorGUILayout.ObjectField(
            "Capture Camera",
            captureCamera,
            typeof(Camera),
            true
        );

        outputFolder = EditorGUILayout.TextField(
            "Output Folder",
            outputFolder
        );

        imageSize = EditorGUILayout.IntPopup(
            "Image Size",
            imageSize,
            new[] { "256 x 256", "512 x 512", "1024 x 1024" },
            new[] { 256, 512, 1024 }
        );

        padding = EditorGUILayout.Slider(
            "Model Padding",
            padding,
            1.0f,
            1.5f
        );

        EditorGUILayout.Space();
        GUILayout.Label("Model Rotation", EditorStyles.boldLabel);

        rotationY = EditorGUILayout.Slider(
            "Horizontal Rotation",
            rotationY,
            -180f,
            180f
        );

        rotationX = EditorGUILayout.Slider(
            "Vertical Rotation",
            rotationX,
            -45f,
            45f
        );

        EditorGUILayout.Space();

        GUI.enabled = !isCapturing;

        if (GUILayout.Button("Capture All Trash Prefabs", GUILayout.Height(35)))
        {
            CaptureAllPrefabs();
        }

        GUI.enabled = true;
    }

    private void CaptureAllPrefabs()
    {
        // setup to create the icons.
        if (prefabFolder == null)
        {
            Debug.LogError("Please assign the folder containing your trash prefabs.");
            return;
        }

        if (captureCamera == null)
        {
            Debug.LogError("Please assign your CaptureCamera.");
            return;
        }

        string folderPath = AssetDatabase.GetAssetPath(prefabFolder);

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError("The selected asset is not a folder.");
            return;
        }

        string[] prefabGuids = AssetDatabase.FindAssets(
            "t:Prefab",
            new[] { folderPath }
        );

        if (prefabGuids.Length == 0)
        {
            Debug.LogWarning("No prefabs were found in " + folderPath);
            return;
        }

        Directory.CreateDirectory(outputFolder);

        // remove existing PNG images from the output folder.
        string[] existingImages = Directory.GetFiles(
            outputFolder,
            "*.png",
            SearchOption.TopDirectoryOnly
        );

        foreach (string imagePath in existingImages)
        {
            File.Delete(imagePath);
        }

        AssetDatabase.Refresh();

        isCapturing = true;

        int successfulCaptures = 0;

        try
        {
            // go through all prefabs and capture and icon for them.
            foreach (string guid in prefabGuids)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                    prefabPath
                );

                if (prefab == null)
                    continue;

                if (CapturePrefab(prefab))
                    successfulCaptures++;
            }
        }
        finally
        {
            isCapturing = false;
            AssetDatabase.Refresh();
        }

        Debug.Log(
            $"Capture finished. Successfully captured " +
            $"{successfulCaptures} of {prefabGuids.Length} prefabs."
        );
    }

    private bool CapturePrefab(GameObject prefab)
    {
        GameObject instance = null;
        RenderTexture renderTexture = null;
        Texture2D image = null;

        try
        {
            // instantiate the prefab into capture scene.
            instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            if (instance == null)
            {
                Debug.LogError($"Could not instantiate {prefab.name}.");
                return false;
            }

            instance.transform.position = Vector3.zero;

            // rotate new instance based on chosen rotation from tool.
            instance.transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);

            // find the visible model bounds.
            Renderer[] renderers = instance.GetComponentsInChildren<Renderer>();

            bool foundBounds = false;
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

            foreach (Renderer renderer in renderers)
            {
                // inactive models should not count.
                if (!renderer.enabled) continue;

                if (!foundBounds)
                {
                    // when its the first bounds we find we just set it.
                    bounds = renderer.bounds;
                    foundBounds = true;
                }
                else
                {
                    // if we have multiple bounds we must combine them together to form a new bound
                    // that fits both bounds.
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            if (!foundBounds)
            {
                // if no models are on prefab / or none are active we skip it.
                Debug.LogWarning($"Skipping {prefab.name}: no enabled Renderers found.");
                return false;
            }

            // use a consistent orthographic view for every prefab.
            captureCamera.orthographic = true;

            Vector3 centre = bounds.center;

            captureCamera.transform.position = centre - captureCamera.transform.forward * 5f;

            captureCamera.orthographicSize = Mathf.Max(bounds.extents.magnitude * padding, 0.05f);

            captureCamera.nearClipPlane = 0.01f;
            captureCamera.farClipPlane = 100f;

            // make sure background is transparent on icon.
            captureCamera.clearFlags = CameraClearFlags.SolidColor;
            captureCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            captureCamera.allowHDR = false;
            captureCamera.allowMSAA = false;

            // create a square render target.
            renderTexture = new RenderTexture(
                imageSize,
                imageSize,
                24,
                RenderTextureFormat.ARGB32
            );

            renderTexture.Create();
            captureCamera.targetTexture = renderTexture;

            // request a render through URP.
            UniversalRenderPipeline.SingleCameraRequest request = new UniversalRenderPipeline.SingleCameraRequest();

            request.destination = renderTexture;

            RenderPipeline.SubmitRenderRequest(captureCamera, request);

            // read the rendered pixels.
            RenderTexture.active = renderTexture;

            image = new Texture2D(imageSize, imageSize, TextureFormat.RGBA32, false);
            image.ReadPixels(new Rect(0, 0, imageSize, imageSize), 0, 0);
            image.Apply();

            string formattedName = prefab.name.Replace(" ", "") + "Icon";

            // save the icon PNG using the prefabs name.
            string imagePath = Path.Combine(outputFolder, formattedName + ".png");

            File.WriteAllBytes(imagePath, image.EncodeToPNG());

            Debug.Log($"Captured {prefab.name}: {imagePath}");

            return true;
        }
        catch (System.Exception exception)
        {
            // if there is any king of error/exception while trying to capture icon.
            // we log it and skip prefab.
            Debug.LogError($"Failed to capture {prefab.name}: {exception.Message}");
            return false;
        }
        finally
        {
            // remove the temporary prefab instance, image and render texture.
            if (instance != null) DestroyImmediate(instance);

            if (image != null) DestroyImmediate(image);

            if (renderTexture != null)
            {
                renderTexture.Release();
                DestroyImmediate(renderTexture);
            }
        }
    }
}