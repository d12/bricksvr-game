using UnityEngine;
 using System.Collections;
 using System.IO;

 // Screen Recorder will save individual images of active scene in any resolution and of a specific image format
 // including raw, jpg, png, and ppm.  Raw and PPM are the fastest image formats for saving.
 //
 // You can compile these images into a video using ffmpeg:
 // ffmpeg -i screen_3840x2160_%d.ppm -y test.avi

 public class CameraScreenshotTest : MonoBehaviour
 {
     private static readonly int Color = Shader.PropertyToID("_Color");
     public int captureWidth = 1920;
     public int captureHeight = 1080;
     public GameObject mainEnv;
     private Camera _camera;

     // configure with raw, jpg, png, or ppm (simple raw format)
     public enum Format { RAW, JPG, PNG, PPM };
     public Format format = Format.PPM;

     // folder to write output (defaults to data path)
     public string folder;
     public Color color;

     // private vars for screenshot
     private Rect _rect;
     private RenderTexture _renderTexture;
     private Texture2D _screenShot;
     public bool takeScreenshot;
     public bool skipSaving;

     // create a unique filename using a one-up variable
     private string UniqueFilename(int width, int height)
     {
         return $"{folder}/{System.Guid.NewGuid()}.png";
     }

     void Update()
     {
         if (!takeScreenshot) return;
         TakeScreenshot();
     }

     private void TakeScreenshot()
     {
         _camera = GetComponent<Camera>();
         takeScreenshot = false;
         mainEnv.SetActive(true);

         // create screenshot objects if needed
         if (_renderTexture == null)
         {
             // creates off-screen render texture that can rendered into
             _rect = new Rect(0, 0, captureWidth, captureHeight);
             _renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
             _screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGBA32, false);
         }

         // get main camera and manually render scene into rt
         _camera.targetTexture = _renderTexture;
         _camera.Render();

         // read pixels will read from the currently active render texture so make our offscreen
         // render texture active and then read the pixels
         RenderTexture.active = _renderTexture;
         if (!skipSaving) _screenShot.ReadPixels(_rect, 0, 0);

         // reset active camera texture and render texture
         RenderTexture.active = null;

         if (skipSaving) return;

         // get our unique filename
         string filename = UniqueFilename((int) _rect.width, (int) _rect.height);

         // pull in our file header/data bytes for the specified image format (has to be done from main thread)
         byte[] fileHeader = null;
         byte[] fileData = null;
         if (format == Format.RAW)
         {
             fileData = _screenShot.GetRawTextureData();
         }
         else if (format == Format.PNG)
         {
             fileData = _screenShot.EncodeToPNG();
         }
         else if (format == Format.JPG)
         {
             fileData = _screenShot.EncodeToJPG();
         }
         else // ppm
         {
             // create a file header for ppm formatted file
             string headerStr = string.Format("P6\n{0} {1}\n255\n", _rect.width, _rect.height);
             fileHeader = System.Text.Encoding.ASCII.GetBytes(headerStr);
             fileData = _screenShot.GetRawTextureData();
         }

         // create new thread to save the image to file (only operation that can be done in background)
         new System.Threading.Thread(() =>
         {
             // create file and write optional header with image bytes
             var f = System.IO.File.Create(filename);
             if (fileHeader != null) f.Write(fileHeader, 0, fileHeader.Length);
             f.Write(fileData, 0, fileData.Length);
             f.Close();
             Debug.Log(string.Format("Wrote screenshot {0} of size {1}", filename, fileData.Length));
         }).Start();

         mainEnv.SetActive(false);
     }

     private void OnValidate()
     {
         if (!Application.isEditor) return;

         MaterialPropertyBlock props = new MaterialPropertyBlock();
         props.SetColor(Color, color);

         Renderer brickRenderer = GetComponentInChildren<Renderer>();
         if(brickRenderer != null)
             brickRenderer.SetPropertyBlock(props);
     }
 }