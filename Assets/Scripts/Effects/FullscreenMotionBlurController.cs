using UnityEngine;

// Attach this to a GameObject with MeshRenderer using the fullscreen triangle mesh
public class FullscreenMotionBlurController : MonoBehaviour
{
    [Range(0f, 1f)]
    public float blurAmount = 0.85f;
    
    [Tooltip("Material using the FullscreenMotionBlur shader")]
    public Material motionBlurMaterial;
    
    private RenderTexture accumTexture;
    private Camera targetCamera;
    
    void Start()
    {
        // Find the main camera
        targetCamera = Camera.main;
        if (targetCamera == null)
            targetCamera = FindFirstObjectByType<Camera>();
            
        if (motionBlurMaterial == null)
        {
            var renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
                motionBlurMaterial = renderer.material;
        }
        
        // Create accumulation texture
        CreateAccumTexture();
    }
    
    void CreateAccumTexture()
    {
        if (targetCamera == null) return;
        
        int width = Screen.width;
        int height = Screen.height;
        
        if (accumTexture != null)
            accumTexture.Release();
            
        accumTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        accumTexture.filterMode = FilterMode.Bilinear;
        accumTexture.Create();
        
        // Clear to black initially
        RenderTexture.active = accumTexture;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = null;
        
        // Assign to material
        if (motionBlurMaterial != null)
            motionBlurMaterial.SetTexture("_AccumTex", accumTexture);
    }
    
    void Update()
    {
        if (motionBlurMaterial != null)
        {
            motionBlurMaterial.SetFloat("_BlurAmount", blurAmount);
        }
        
        // Recreate texture if screen size changed
        if (accumTexture != null && (accumTexture.width != Screen.width || accumTexture.height != Screen.height))
        {
            CreateAccumTexture();
        }
    }
    
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (accumTexture == null || motionBlurMaterial == null)
        {
            Graphics.Blit(source, destination);
            return;
        }
        
        // Update accumulation texture with current frame
        Graphics.Blit(source, accumTexture, motionBlurMaterial);
        
        // Output the accumulated result
        Graphics.Blit(accumTexture, destination);
    }
    
    void OnDestroy()
    {
        if (accumTexture != null)
        {
            accumTexture.Release();
            accumTexture = null;
        }
    }
    
    void OnValidate()
    {
        blurAmount = Mathf.Clamp01(blurAmount);
    }
}
