using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : NetworkBehaviour {
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    [SerializeField] private Button [] testCloneBtn;
    [SerializeField] private GameObject testClone , test_2_Obj , test_2_Obj_Test , TestColorObject;

    [SerializeField] private GameObject testPlane;    
    [SerializeField] private Text showText;

    // private NetworkObjectReference spawnedTestObjectReference = null;

    private NetworkVariable<int> playersNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    private void Awake() {


        serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
        });
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });
        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
        testCloneBtn[0].onClick.AddListener(() => {
            TestCloneObject();
        });
                // Setname("Hello123");
        testCloneBtn[1].onClick.AddListener(() => {
            // MakeObjectClientRpc();  
            ChangeScene();
        });
        testCloneBtn[2].onClick.AddListener(() => {
            // MakeObjectClientRpc();  
            ChangeColor();
        });
    }
    // Start is called before the first frame update
    void Start()
    {
        
        // if (!IsServer){
        //     SetnameClientRpc("ReWrithCloneName");
        // }
    }

    // Update is called once per frame
    void Update()
    {
        showText.text  = "Players: " +playersNum.Value.ToString(); // *1

        if(!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            // TestingClientRpc();
            TestServerRpc();
        }

        // *1
        if (!IsServer) return;
        playersNum.Value =NetworkManager.Singleton.ConnectedClients.Count;
        //
    }
    // public void Setname(string name){
        // SetnameClientRpc(name);
    // }
    
    public void TestCloneObject(){
        if (IsHost){
            // MakeObjectServerRpc();
            TestCloneFunc();
        }else{
            
            Debug.Log("Launch on TestCloneObject");
            // MakeObjectClientRpc();
        }
    }

    public void TestCloneFunc(){
        //  Instantiate(testClone);
        GameObject spawnedTestOject = Instantiate(testClone);
        spawnedTestOject.GetComponent<NetworkObject>().Spawn(true);
        
        // ObjectColorChange ObjChangeC = spawnedTestOject.GetComponent<ObjectColorChange>();
        // ObjChangeC.changeColorFunc_Green();

        Color c_new = Color.red;
        ChangeColorFunc_Green_ServerRpc(c_new);
    }




    [ServerRpc(RequireOwnership = false)] // Allow clients to request a scene change
    private void RequestSceneChangeServerRpc(string sceneName)
    {
        // Only allow the server to load the scene
        if (IsServer)
        {
            SceneManager.LoadScene(sceneName);
            ChangeSceneClientRpc(sceneName); // Inform clients about the scene change
        }
    }

    [ClientRpc]
    private void ChangeSceneClientRpc(string sceneName)
    {
        if (!IsServer)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private void ChangeScene()
    {
        string sceneName = "TestScene";

        if (IsServer)
        {
            SceneManager.LoadScene(sceneName);
            ChangeSceneClientRpc(sceneName); // Inform clients about the scene change
        }
        else if (IsClient)
        {
            RequestSceneChangeServerRpc(sceneName); // Request the scene change from the server
        }
    }




    [ServerRpc]
    private void MakeObjectServerRpc(){
        
        Debug.Log("Launch on Server");
        TestCloneFunc();
    }

    [ServerRpc()]
    private void TestServerRpc(){   
        Debug.Log("-- TestServerRpc "+ OwnerClientId);
    }
    
    [ServerRpc]
    private void ChangeColorFunc_Green_ServerRpc(Color _c){
        Debug.Log("Launch on Server");
        // ObjectColorChange ObjChangeC = obj.GetComponent<ObjectColorChange>();
        // ObjChangeC.renderer.material.SetColor("_Color", _c);
    }

    [ClientRpc]
    private void MakeObjectClientRpc( )
    {
        Debug.Log("Launch on ClientRpc");
        test_2_Obj_Test = test_2_Obj;   

        ObjectColorChange ObjChangeC = test_2_Obj_Test.GetComponent<ObjectColorChange>();
        // ObjChangeC.changeColorFunc_Red();
        ObjChangeC.renderer.material.SetColor("_Color", Color.red);

    }



    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Change the color of the object
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void ChangeObjectColor(Color newColor)
    {
        Renderer renderer = TestColorObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = newColor;
        }
    }

    [ServerRpc(RequireOwnership = false)] // Allow clients to request a ChangeColorServerRpc
    private void ChangeColorServerRpc(Color newColor)
    {
        // 在主机上进行颜色更改逻辑
        if (IsServer)
        {
            ChangeColorClientRpc(newColor);
        }
    }

    [ClientRpc]
    private void ChangeColorClientRpc(Color newColor)
    {
        // 在客户端上进行颜色更改逻辑
        ChangeObjectColor(newColor);
    }

    private void ChangeColor()
    {
        // if (IsServer)
        // {
        //     ChangeColorServerRpc();
        // }
        // else if (IsClient)
        // {
        //     ChangeColorClientRpc(Color.blue);
        // }
        if (IsServer)
        {
            // ChangeColorServerRpc();
            ChangeColorClientRpc(Color.red);
        }
        else if (IsClient)
        {
            ChangeColorServerRpc(Color.blue);
        }
    }
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    // [ClientRpc]
    // private void MakeObjectClientRpc( )
    // {
    //     Debug.Log("Launch on ClientRpc");
    //     if (IsOwner) return;
    //     TestCloneFunc();
    // }
    // [ClientRpc]
    // public void SetnameClientRpc(string name){
    //     Debug.Log("--SetnameClientRpc");
    //     testPlane.name = name;
    // }

    // [ClientRpc]
    // private void TestingClientRpc( )
    // {
    //     Debug.Log("-- is TestingClientRpc");
    // }



}
