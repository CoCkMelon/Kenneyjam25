using UnityEngine;

// Simple motion blur controller that works with any fullscreen shader
// Attach to a camera or use as a singleton
public class SimpleMotionBlurController : MonoBehaviour
{
    [Header("Motion Blur Settings")]
    [Range(0f, 1f)]
    public float blurAmount = 0.85f;
    
    [Header("References")]
    public Material fullscreenMaterial;
    public Camera targetCamera;
    
    private RenderTexture accumTexture;
    private RenderTexture tempTexture;
    
    void Start()
    {
        if (targetCamera == null)
            targetCamera = GetComponent<Camera>();
        if (targetCamera == null)
            targetCamera = Camera.main;
            
        CreateRenderTextures();
    }
    
    void CreateRenderTextures()
    {
        int width = Screen.width;
        int height = Screen.height;
        
        // Release existing textures
        if (accumTexture != null) accumTexture.Release();
        if (tempTexture != null) tempTexture.Release();
        
        // Create new textures
        accumTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        tempTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        
        accumTexture.filterMode = FilterMode.Bilinear;
        tempTexture.filterMode = FilterMode.Bilinear;
        
        accumTexture.Create();
        tempTexture.Create();
        
        // Clear accumulation buffer
        RenderTexture.active = accumTexture;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = null;
        
        // Set material properties
        if (fullscreenMaterial != null)
        {
            fullscreenMaterial.SetTexture("_AccumTexture", accumTexture);
            fullscreenMaterial.SetFloat("_BlurAmount", blurAmount);
        }
    }
    
    void Update()
    {
        // Update blur amount
        if (fullscreenMaterial != null)
        {
            fullscreenMaterial.SetFloat("_BlurAmount", blurAmount);
        }
        
        // Recreate textures if screen size changed
        if (accumTexture != null && 
            (accumTexture.width != Screen.width || accumTexture.height != Screen.height))
        {
            CreateRenderTextures();
        }
    }
    
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (fullscreenMaterial == null || accumTexture == null)
        {
            Graphics.Blit(source, destination);
            return;
        }
        
        // Blend current frame with accumulation
        fullscreenMaterial.SetTexture("_MainTex", source);
        fullscreenMaterial.SetTexture("_AccumTexture", accumTexture);
        fullscreenMaterial.SetFloat("_BlurAmount", blurAmount);
        
        // Render to temp texture
        Graphics.Blit(source, tempTexture, fullscreenMaterial);
        
        // Copy temp to accumulation for next frame
        Graphics.Blit(tempTexture, accumTexture);
        
        // Output final result
        Graphics.Blit(tempTexture, destination);
    }
    
    void OnDestroy()
    {
        if (accumTexture != null)
        {
            accumTexture.Release();
            accumTexture = null;
        }
        
        if (tempTexture != null)
        {
            tempTexture.Release();
            tempTexture = null;
        }
    }
    
    void OnValidate()
    {
        blurAmount = Mathf.Clamp01(blurAmount);
    }
}
