using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectColorChange : MonoBehaviour
{
    // public Color color;

    public Renderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void changeColorFunc_Red (){
        // Color _c = new Color.RED;
        renderer.material.SetColor("_Color", Color.red);
        //     renderer.material.color = _c;
    }
    public void changeColorFunc_Green(){
        
        renderer.material.SetColor("_Color", Color.green);
        print("changed box color to green");
    }

}
