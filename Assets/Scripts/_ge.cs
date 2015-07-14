using UnityEngine;
using System.Collections;

public class _ge : MonoBehaviour {

    public Transform tp1, tp2;
    public Texture[] textures_ = new Texture[11];
    public int xfg_;
    
	void Start () {
		//标记坐标
        // tp1.renderer.material.mainTexture = textures_[10];
        // tp2.renderer.material.mainTexture = textures_[5];
        if (xfg_ < 10)
        {
            tp1.GetComponent<Renderer>().material.mainTexture = textures_[10]; // 十位 空
            tp2.GetComponent<Renderer>().material.mainTexture = textures_[xfg_]; // 个位 数字
        }
        else if (xfg_ >= 10 && xfg_ <= 99)
        {
            int sw_ = (int)(xfg_ / 10); // 十位
            int gw_ = xfg_ - sw_ * 10; // 个位
            tp1.GetComponent<Renderer>().material.mainTexture = textures_[sw_];
            tp2.GetComponent<Renderer>().material.mainTexture = textures_[gw_];
        }
	}
	
	void Update () {
	
	}
}
