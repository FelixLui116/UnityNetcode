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

    // private NetworkObjectReference spawnedTestObjectReference;
    [SerializeField]  private NetworkObject spawnedTestObject;

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

    /////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////TestCloneFunc
    
    public void TestCloneObject(){
       
        // TestCloneFunc();
        if (IsServer)
        {
            TestCloneFunc();
        }
        else
        {
            Debug.Log("Launch on TestCloneObject");
            TestCloneObjectServerRpc();
        }
    }

    public void TestCloneFunc(){
        // GameObject spawnedTestObject = Instantiate(testClone);
        // spawnedTestObject.GetComponent<NetworkObject>().Spawn(true);
        // spawnedTestObjectReference = spawnedTestObject.GetComponent<NetworkObject>();
        
        // Debug.Log(": "+ );
        GameObject spawnedTestObject_clone = Instantiate(testClone);

        NetworkObject networkObject = spawnedTestObject_clone.GetComponent<NetworkObject>();
        // networkObject.SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
        networkObject.Spawn(true);
        ulong networkObjectId = networkObject.NetworkObjectId;
        Debug.Log("Network Object ID: " + networkObjectId);

        AddObjectToClient(networkObjectId);
    }

    // private void AddObject_func(NetworkObjectReference tarage_obj, NetworkObjectReference  networkObject){
    //     tarage_obj = networkObject;
    // }

    private void AddObjectToClient(ulong networkObjectId){
        // if (IsServer)
        // {
        //     AddObjectClientRpc(networkObjectId);
        // }
        // else if (IsClient)
        // {
        //     AddObjectServerRpc(networkObjectId);
        // }
        AddObjectClientRpc(networkObjectId);
    }
    [ServerRpc(RequireOwnership = false)] // Allow client to request object cloning
    public void TestCloneObjectServerRpc()
    {
        TestCloneFunc();
    }

    [ServerRpc(RequireOwnership = false)] // Allow client to add object to server
    public void AddObjectServerRpc(ulong  tarage_obj)
    {
        // AddObjectClientRpc(tarage_obj, networkObject);
    }
    // [ClientRpc]
    // public void AddObjectClientRpc(NetworkObjectReference  tarage_obj, NetworkObjectReference  networkObject)
    // {
    //     AddObject_func(tarage_obj, networkObject);
    // }
    [ClientRpc]
    public void AddObjectClientRpc(ulong networkObjectId)
    {
        // 在客户端中根据网络标识符查找相应的网络对象
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject clientNetworkObject))
        {
            // 在客户端上操作已找到的网络对象
            // 使用 clientNetworkObject 进行进一步操作
            Debug.Log("Received Network Object on client: " + clientNetworkObject.NetworkObjectId);
            spawnedTestObject = clientNetworkObject;
        }
    }





    //////////////////////////////////////////////////////////////////////////////////
    ////////// change scene

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


    private void ChangeObjectColor(Color newColor)
    {
        // Renderer renderer = TestColorObject.GetComponent<Renderer>();
        Renderer renderer = spawnedTestObject.GetComponent<Renderer>();
        
        // NetworkObject spawnedTestObject = spawnedTestObjectReference.Value;
        // Renderer renderer = spawnedTestObject.GetComponent<Renderer>();

        renderer.material.color = newColor;
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
