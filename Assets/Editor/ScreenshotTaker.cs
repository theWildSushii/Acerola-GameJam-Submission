using System.IO;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Screenshot : EditorWindow {

    public Camera targetCamera;

    int resWidth = 1920;
    int resHeight = 1080;
    string path = "";
    bool takeShotFromCamera = false;
    bool takeShotFromGameView = false;

    [MenuItem("Window/Desert Hare Studios/Screenshot Tool")]
    public static void ShowWindow() {
        EditorWindow editorWindow = GetWindow(typeof(Screenshot));
        editorWindow.autoRepaintOnSceneChange = true;
        editorWindow.Show();
        editorWindow.titleContent = new GUIContent("Screenshot");
    }

    void OnGUI() {
        if(string.IsNullOrEmpty(path)) {
            path = Path.Combine(Application.dataPath, "../Screenshots");
            path = Path.GetFullPath(path);
        }
        GUILayout.Label("Save Path", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField(path, GUILayout.ExpandWidth(false));
        if(GUILayout.Button("Browse", GUILayout.ExpandWidth(false))) {
            path = EditorUtility.SaveFolderPanel("Path to Save Images", path, Application.dataPath);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(16);
        if(GUILayout.Button("Take Screenshot From GameView", GUILayout.MinHeight(32))) {
            if(string.IsNullOrEmpty(path)) {
                path = EditorUtility.SaveFolderPanel("Path to Save Images", path, Application.dataPath);
                Debug.Log("Path Set");
                TakeShotFromGameView();
            } else {
                TakeShotFromGameView();
            }
        }
        GUILayout.Space(16);
        targetCamera = EditorGUILayout.ObjectField(targetCamera, typeof(Camera), true, null) as Camera;
        if(targetCamera == null) {
            targetCamera = Camera.main;
        }
        EditorGUILayout.LabelField("Resolution", EditorStyles.boldLabel);
        resWidth = EditorGUILayout.IntField("Width", resWidth);
        resHeight = EditorGUILayout.IntField("Height", resHeight);
        GUILayout.Label("Select Camera", EditorStyles.boldLabel);
        if(GUILayout.Button("Take Screenshot From Scene", GUILayout.MinHeight(32))) {
            if(string.IsNullOrEmpty(path)) {
                path = EditorUtility.SaveFolderPanel("Path to Save Images", path, Application.dataPath);
                TakeShotFromCamera();
            } else {
                TakeShotFromCamera();
            }
        }
        if(string.IsNullOrEmpty(path)) {
            return;
        }
        if(takeShotFromCamera) {
            int resWidthN = resWidth;
            int resHeightN = resHeight;
            RenderTexture rt = new RenderTexture(resWidthN, resHeightN, 24);
            targetCamera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidthN, resHeightN, TextureFormat.RGBA32, false);
            targetCamera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
            targetCamera.targetTexture = null;
            RenderTexture.active = null;
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName;
            if(!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeShotFromCamera = false;
        }
        if(takeShotFromGameView) {
            ScreenCapture.CaptureScreenshot(ScreenShotName);
            takeShotFromGameView = false;
        }
    }

    public string ScreenShotName {
        get {
            return string.Format("{0}/screen_{1}.png",
                path,
                System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        }
    }

    public void TakeShotFromCamera() {
        takeShotFromCamera = true;
    }

    public void TakeShotFromGameView() {
        takeShotFromGameView = true;
    }

}
