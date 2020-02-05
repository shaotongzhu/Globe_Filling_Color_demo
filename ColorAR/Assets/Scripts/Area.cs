using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour {

    private Vector3 TopLeft_Pl_W;
    //申请Vector3类型的变量 记录面片左上角的世界坐标
    private Vector3 BottomLeft_Pl_W;
    //申请Vector3类型的变量 记录面片左下角的世界坐标
    private Vector3 TopRight_Pl_W;
    //申请Vector3类型的变量 记录面片右上角的世界坐标
    private Vector3 BottomRight_Pl_W;
    //申请Vector3类型的变量 记录面片右下角的世界坐标

    public GameObject Card_Track;
    //申请GameObject类型的变量 储存识别图
    private Vector3 Center_Card;
    //申请Vector3类型的变量 储存识别图的世界坐标
    private float Half_W;
    //申请float类型的变量储存识别图宽的一半
    private float Half_H;
    //申请float类型的变量储存识别图高的一半

    public GameObject Earth;
    //记录地球模型
    public GameObject Frame;
    //记录地球外框模型

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Get_Position() {

        //获取坐标位置
        Center_Card = Card_Track.transform.position;
        //获取识别图的世界坐标
        Half_W = Card_Track.GetComponent<MeshFilter>().mesh.bounds.size.x*10*0.5f;
        //获取识别图宽的一半 注意10是识别图缩放的系数
        Half_H = Card_Track.GetComponent<MeshFilter>().mesh.bounds.size.z*10*0.5f;
        //获取识别图高的一半 注意10是识别图缩放的系数
        TopLeft_Pl_W = Center_Card + new Vector3(- Half_W, 0, Half_H);     
        //计算识别图左上角的世界坐标 
        BottomLeft_Pl_W = Center_Card + new Vector3(-Half_W, 0, -Half_H);
        //计算识别图左下角的世界坐标
        TopRight_Pl_W = Center_Card + new Vector3(Half_W, 0, Half_H);
        //计算识别图右上角的世界坐标
        BottomRight_Pl_W = Center_Card + new Vector3(Half_W, 0, -Half_H);
        //计算识别图右下角的世界坐标

        //将截图时识别图四个角的世界坐标信息传递给地球模型的Shader
        Earth.GetComponent<Renderer>().material.SetVector("_Uvpoint1", new Vector4(TopLeft_Pl_W.x, TopLeft_Pl_W.y, TopLeft_Pl_W.z, 1f));
        //将左上角的世界坐标传递给Shader ，其中1f是为了凑齐四位浮点数 ，用来进行后续的矩阵变换操作
        Earth.GetComponent<Renderer>().material.SetVector("_Uvpoint2", new Vector4(BottomLeft_Pl_W.x, BottomLeft_Pl_W.y, BottomLeft_Pl_W.z, 1f));
        Earth.GetComponent<Renderer>().material.SetVector("_Uvpoint3", new Vector4(TopRight_Pl_W.x, TopRight_Pl_W.y, TopRight_Pl_W.z, 1f));
        Earth.GetComponent<Renderer>().material.SetVector("_Uvpoint4", new Vector4(BottomRight_Pl_W.x, BottomRight_Pl_W.y, BottomRight_Pl_W.z, 1f));

        //将截图时识别图四个角的世界坐标信息传递给地球仪配件模型的Shader
        Frame.GetComponent<Renderer>().material.SetVector("_Uvpoint1", new Vector4(TopLeft_Pl_W.x, TopLeft_Pl_W.y, TopLeft_Pl_W.z, 1f));
        //将左上角的世界坐标传递给Shader ，其中1f是为了凑齐四位浮点数 ，用来进行后续的矩阵变换操作
        Frame.GetComponent<Renderer>().material.SetVector("_Uvpoint2", new Vector4(BottomLeft_Pl_W.x, BottomLeft_Pl_W.y, BottomLeft_Pl_W.z, 1f));
        Frame.GetComponent<Renderer>().material.SetVector("_Uvpoint3", new Vector4(TopRight_Pl_W.x, TopRight_Pl_W.y, TopRight_Pl_W.z, 1f));
        Frame.GetComponent<Renderer>().material.SetVector("_Uvpoint4", new Vector4(BottomRight_Pl_W.x, BottomRight_Pl_W.y, BottomRight_Pl_W.z, 1f));

        //确定坐标间的矩阵关系 并将信息传递给对应模型的shader
        Matrix4x4 P = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false);
        //获取截图时GPU的投影矩阵
        Matrix4x4 V = Camera.main.worldToCameraMatrix;
        //获取截图时世界坐标到相机的矩阵
        Matrix4x4 VP = P * V;
        //储存两个矩阵的乘积
        Earth.GetComponent<Renderer>().material.SetMatrix("_VP", VP);
        //将截图时的矩阵转换信息传递给Shader
        Frame.GetComponent<Renderer>().material.SetMatrix("_VP", VP);
        //将截图时的矩阵转换信息传递给Shader

    }

    public void ScreenShot() {
        Texture2D Te = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        //申请Texture2D类型的变量宽高为（Screen.width, Screen.height）
        //颜色模式为TextureFormat.RGB24
        //不适用mipmap

        Te.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        //用Texture2D类型的变量Te来读取屏幕像素
        //读取的起始点为屏幕的（0,0）点，读取的宽高为屏幕的宽高
        //将读取到的屏幕图像从Te的（0,0）点开始填充

        Te.Apply();
        //执行对Texture2D的操作

        Earth.GetComponent<Renderer>().material.mainTexture = Te;
        //将地球模型材质的主贴图替换为屏幕截图
        Frame.GetComponent<Renderer>().material.mainTexture = Te;
        //将地球仪配件模型材质的主贴图替换为屏幕截图
    }

    public void Button_Color() {
        ScreenShot();
        //调用截屏函数
        Get_Position();
        //调用坐标获取函数
    }
}
