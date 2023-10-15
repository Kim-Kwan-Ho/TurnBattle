using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Managers : MonoBehaviour
{

    private static Managers s_instance = null;
    public static Managers Instance { get {Init(); return s_instance; } }

    private UIManager _ui = new UIManager();
    public static UIManager UI { get {return Instance._ui; } }


    private ResourceManager _resource = new ResourceManager();
    public static ResourceManager Resource { get {  return Instance._resource; } }


    private MySceneManager _scene = new MySceneManager();
    public static MySceneManager Scene { get { return Instance._scene; } }


    private DataManager _data = new DataManager();
    public static DataManager Data { get { return Instance._data; } }
    private NetworkManager _network = new NetworkManager();
    public static NetworkManager Network { get { return Instance._network; } }

    private BattleManager _battle = new BattleManager();
    public static BattleManager Battle { get { return Instance._battle; } }
    private void Start()
    {
        Init();
    }
    private static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
                go = new GameObject("@Managers");

            s_instance = Utils.GetOrAddComponent<Managers>(go);
            DontDestroyOnLoad(go);
            Application.targetFrameRate = 60;

            s_instance._scene.Init();
            s_instance._resource.Init();
            s_instance._network.Init();

        }
    }

    private void OnApplicationQuit()
    {
        Managers.Instance._network.DisConnect();
    }


}
