using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.Networking;

public class NetworkManagerUI : NetworkBehaviour {
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    
    [SerializeField] private Button testCloneBtn;
    [SerializeField] private GameObject testClone;

    [SerializeField] private GameObject testPlane;    
    [SerializeField] private Text showText;
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
        testCloneBtn.onClick.AddListener(() => {
            TestCloneObject();
                // Setname("Hello123");
            
            
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
        if(!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            // TestingClientRpc();
            TestServerRpc();
        }
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
