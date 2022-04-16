using UnityEngine;

public class RenderReplacementShaderToTexture : MonoBehaviour
{
    [SerializeField]
    Shader replacementShader;

    [SerializeField]
    RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32;

    [SerializeField]
    FilterMode filterMode = FilterMode.Point;

    [SerializeField]
    int renderTextureDepth = 24;

    [SerializeField]
    CameraClearFlags cameraClearFlags = CameraClearFlags.Color;

    [SerializeField]
    Color background = Color.black;

    [SerializeField]
    string targetTexture = "_RenderTexture";

    private RenderTexture renderTexture;
    private new Camera camera;

	protected Camera thisCamera;
	protected int pixelWidth;
	protected int pixelHeight;

    private void Start()
    {
		//foreach (Transform t in transform) {
		//	DestroyImmediate(t.gameObject);
		//}

		thisCamera = GetComponent<Camera>();

		// Create a render texture matching the main camera's current dimensions.
		//renderTexture = new RenderTexture(thisCamera.pixelWidth, thisCamera.pixelHeight, renderTextureDepth, renderTextureFormat);
		pixelWidth = thisCamera.pixelWidth;
		pixelHeight = thisCamera.pixelHeight;

		////renderTexture = CreateRenderTextureFromCamera(camera);
		////renderTexture.filterMode = filterMode;
        
		// Surface the render texture as a global variable, available to all shaders.
        ////Shader.SetGlobalTexture(targetTexture, renderTexture);

        // Setup a copy of the camera to render the scene using the normals shader.
        GameObject copy = new GameObject("Camera" + targetTexture);
        camera = copy.AddComponent<Camera>();
        camera.CopyFrom(thisCamera);
        camera.transform.SetParent(transform);
		//camera.targetTexture = renderTexture;

		ConfigureRenderTexture(camera);

		camera.SetReplacementShader(replacementShader, "RenderType");
        camera.depth = thisCamera.depth - 1;
        camera.clearFlags = cameraClearFlags;
        camera.backgroundColor = background;
    }

	protected void ConfigureRenderTexture(Camera camera)
	{
		RenderTexture newTex = new RenderTexture(pixelWidth, pixelHeight, renderTextureDepth, renderTextureFormat);

		newTex.filterMode = filterMode;
		Shader.SetGlobalTexture(targetTexture, newTex);
		camera.targetTexture = newTex;

		renderTexture = newTex;

	}

	protected void Update()
	{

		CheckCameraResolutionAndUpdate();
		
	}

	protected void CheckCameraResolutionAndUpdate()
	{
		if (!thisCamera)
			return;

		if (thisCamera.pixelHeight == pixelHeight && thisCamera.pixelWidth == pixelWidth)
			return;

		pixelHeight = thisCamera.pixelHeight;
		pixelWidth = thisCamera.pixelWidth;

		ConfigureRenderTexture(camera);
	}
}
