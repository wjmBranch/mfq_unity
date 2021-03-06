﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Text;

//场景交互的信息
public class SenceInteractiveInfo : MonoBehaviour
{

    public enum SceneType
    {
        大场景=0,
        外景=1,
        平层=2,
        loft=3,
        独栋=4,
        联排=5,
        叠拼=6
    }

    public SceneType sceneType;
   
    public string sceneName;
    public Transform websky;
    public Transform meshRoot;
    public TouchCtrl touchCtrl;

    public CameraUniversalCenter cameraUniversalCenter;
    public LouPanManager louPanManager;

    public InteractiveAction f3d_Home;
    public InteractiveAction f3d_Area;
    public InteractiveAction f3d_Intro;
    public InteractiveAction f3d_Supports;
    public InteractiveAction f3d_Traffic;

    public InteractiveAction[] f3d_HXFBGroup;
    public HuXingType huXingType;

    [HideInInspector]
    public Material hxfbMat;

    Transform lastNeedDisplayRoot;



    public void ProcessInteractiveAction( InteractiveAction i)
    {
        if (lastNeedDisplayRoot != null)
        {
            lastNeedDisplayRoot.gameObject.SetActive(false);
        }
        if (i.cameraUniversal != null)
        {
            cameraUniversalCenter.ChangeCamera(i.cameraUniversal,1);
        }
        if (i.cameraStates != "")
        {
            cameraUniversalCenter.currentCamera.SetCameraPositionAndXYZCount(i.cameraStates);
        }
        if (i.needDisplayRoot != null)
        {
            i.needDisplayRoot.gameObject.SetActive(true);
            lastNeedDisplayRoot = i.needDisplayRoot;
        }
    }

    //简介,配套,交通
    public void ProcessMainBtnAction(int toInt)
    {
        switch (toInt)
        {
            case 0:
                ProcessInteractiveAction(f3d_Home);
                break;

            case 1:
                ProcessInteractiveAction(f3d_Area);
                break;

            case 2:
                ProcessInteractiveAction(f3d_Intro);
                break;

            case 3:
                ProcessInteractiveAction(f3d_Supports);
                break;

            case 4:
                ProcessInteractiveAction(f3d_Traffic);
                break;
        }    
    }


    public bool ProcessHXFBAction(string hxName)
    {

        if (hxName == "")
        {
            ProcessInteractiveAction(f3d_Home);
            return true;
        }

        foreach (InteractiveAction i in f3d_HXFBGroup)
        {
            if (i.needDisplayRoot.name == hxName)
            {
                ProcessInteractiveAction(i);
                return true;
            }
        }

        return false;
    
    }


    //首页     Framework3d.Home();
    //简介	   Framework3d.Intro();
    //区位	   Framework3d.Area();
    //交通	   Framework3d.Traffic();
    //配套	   Framework3d.Supports();
    //户型	   Framework3d.HXFB(hx_name);
    //鸟瞰	   Framework3d.NK();
    //人视	   Framework3d.MY();


    SceneInteractiveManger sceneInteractiveManger;

    [HideInInspector]
    public string senceInteractiveInfoJson;

    void Start()
    {
       //以下目前版本2017.2.0b4会崩溃
       // GameObject g = GameObject.Find("GlobalManager");
        if (sceneInteractiveManger == null)
        {
            GameObject g = GameObject.Find("GlobalManager");
            if (g!=null)
            sceneInteractiveManger =g.GetComponent<SceneInteractiveManger>();

            if (sceneInteractiveManger != null)
            {
                sceneInteractiveManger.AddSenceInteractiveInfo(this);   
            }
        }

    }



    public void RecordInfo()
    {
        senceInteractiveInfoJson = "{";

        if (cameraUniversalCenter != null && cameraUniversalCenter.currentCamera != null)
        {
            senceInteractiveInfoJson += RecordCameraCenterInfo(cameraUniversalCenter);
        }
        else
        {
            string log = gameObject.name + "cameraUniversalCenter未设置！";
            GlobalDebug.Addline(log);
            Debug.Log(log);
            Debug.LogWarning(log);
            Debug.LogError(log);
        }


            senceInteractiveInfoJson += ",";
            senceInteractiveInfoJson += "\"homeInAct\":";
            senceInteractiveInfoJson += RecordInteractiveActionInfo(f3d_Home);
        
            senceInteractiveInfoJson += ",";
            senceInteractiveInfoJson += "\"introInAct\":";
            senceInteractiveInfoJson += RecordInteractiveActionInfo(f3d_Intro);
        
            senceInteractiveInfoJson += ",";
            senceInteractiveInfoJson += "\"areaInAct\":";
            senceInteractiveInfoJson += RecordInteractiveActionInfo(f3d_Area);

            senceInteractiveInfoJson += ",";
            senceInteractiveInfoJson += "\"trafficInAct\":";
            senceInteractiveInfoJson += RecordInteractiveActionInfo(f3d_Traffic);
        
            senceInteractiveInfoJson += ",";
            senceInteractiveInfoJson += "\"supportsInAct\":";
            senceInteractiveInfoJson += RecordInteractiveActionInfo(f3d_Supports);
        

        if (f3d_HXFBGroup.Length > 0)
        {
            senceInteractiveInfoJson += ",";
            senceInteractiveInfoJson += "\"HXFBGroup\":[";

            for (int i = 0; i < f3d_HXFBGroup.Length; i++)
            {
 //               senceInteractiveInfoJson += "{\"";
                if (f3d_HXFBGroup[i].needDisplayRoot != null)
                {
//                  senceInteractiveInfoJson += f3d_HXFBGroup[i].needDisplayRoot.name + "\":";
                    senceInteractiveInfoJson += RecordInteractiveActionInfo(f3d_HXFBGroup[i]);
                }
                else
                {
                    string log = gameObject.name + "户型分布模型没有设置！";
                    GlobalDebug.Addline(log);
                    Debug.Log(log);
                    Debug.LogWarning(log);
                    Debug.LogError(log);
                }
//                senceInteractiveInfoJson += "}";
                if (i != f3d_HXFBGroup.Length - 1)
                {
                    senceInteractiveInfoJson += ",";
                }  
            }

            senceInteractiveInfoJson += "]";

        }

        if (huXingType.allFloor.Length > 0)
        {

            huXingType.RecordEachFloorInteractiveAction();

            senceInteractiveInfoJson += ",";
            senceInteractiveInfoJson += "\"floorGroup\":[";

            for (int i = 0; i < huXingType.allFloor.Length; i++)
            {


        //                senceInteractiveInfoJson += "{";
                        if (huXingType.allFloor[i].interactiveAction.needDisplayRoot != null)
                        {
        //                 senceInteractiveInfoJson += f3d_FloorGroup[i].needDisplayRoot.name + "\":";
                           senceInteractiveInfoJson += RecordInteractiveActionInfo(huXingType.allFloor[i].interactiveAction);
                        }
                        else
                        {
                            string log = gameObject.name + "楼层模型没有设置！";
                            Debug.Log(log);
                            Debug.LogWarning(log);
                            Debug.LogError(log);
                        }
         //               senceInteractiveInfoJson += "}";
                        if (i != huXingType.allFloor.Length - 1)
                        {
                            senceInteractiveInfoJson += ",";
                        }
                    
            }
            senceInteractiveInfoJson += "]";

        }






        /*

                if (f3d_FloorGroup.Length > 0)
                {
                    senceInteractiveInfoJson += ",";
                    senceInteractiveInfoJson += "\"floorGroup\":[";

                    for (int i = 0; i < f3d_FloorGroup.Length; i++)
                    {
        //                senceInteractiveInfoJson += "{";
                        if (f3d_FloorGroup[i].needDisplayRoot != null)
                        {
        //                    senceInteractiveInfoJson += f3d_FloorGroup[i].needDisplayRoot.name + "\":";
                            senceInteractiveInfoJson += RecordInteractiveActionInfo(f3d_FloorGroup[i]);
                        }
                        else
                        {
                            string log = gameObject.name + "楼层模型没有设置！";
                            Debug.Log(log);
                            Debug.LogWarning(log);
                            Debug.LogError(log);
                        }
         //               senceInteractiveInfoJson += "}";
                        if (i != f3d_FloorGroup.Length - 1)
                        {
                            senceInteractiveInfoJson += ",";
                        }
                    }
                    senceInteractiveInfoJson += "]";

                }
        */
        senceInteractiveInfoJson += "}";




    }

    string RecordCameraCenterInfo(CameraUniversalCenter cameraUniversalCenter)
    {

        string jsonString = "\"cameraUniversalCenter\":{";
        jsonString += "\"initialCameraName\":\"" + cameraUniversalCenter.currentCamera.name+"\"";
        jsonString += ",\"cameras\":{";

        for (int i = 0; i < cameraUniversalCenter.cameras.Count; i++)
        {
            CameraUniversal cameraUniversal = cameraUniversalCenter.cameras[i].GetComponent<CameraUniversal>();

            if (cameraUniversal == null)
            {
                string log = cameraUniversalCenter.cameras[i].transform.name + "没有CameraUniversal脚本！";
                GlobalDebug.Addline(log);
                Debug.Log(log);
                Debug.LogWarning(log);
                Debug.LogError(log);
            }

//            jsonString += "{";
            jsonString += "\"" + cameraUniversalCenter.cameras[i].gameObject.name + "\":";
            jsonString += "{";
            jsonString += "\"fieldOfView\":" + cameraUniversalCenter.cameras[i].GetComponent<Camera>().fieldOfView;
            jsonString += ",\"nearClip\":" + cameraUniversalCenter.cameras[i].GetComponent<Camera>().nearClipPlane;
            jsonString += ",\"farClip\":" + cameraUniversalCenter.cameras[i].GetComponent<Camera>().farClipPlane;

            jsonString += ",\"cameraStates\":[" + cameraUniversal.camBase.transform.position.x + ",";
            jsonString += cameraUniversal.camBase.transform.position.y + ",";
            jsonString += -cameraUniversal.camBase.transform.position.z + ",";
            jsonString += cameraUniversal.Xcount + ",";
            jsonString += cameraUniversal.Ycount + ",";

            //i==0为鸟瞰相机,鸟瞰相机多加3米远
            float zFinal = cameraUniversal.Zcount;
            if (i == 0)
                zFinal += 3;
            jsonString += zFinal + ",";

            Vector3 lookAt = new Vector3(0, 0, 0);

/*
            if (GetCameraType(cameraUniversalCenter.cameras[i]) == 1)
            {
                lookAt = cameraUniversal.GetComponent<SpaceCamera>().spaceLookAtObject.transform.position;
//                Debug.Log(lookAt);
                lookAt.z = -lookAt.z;
                Debug.Log(lookAt);
            }
*/
            jsonString += lookAt.x + ",";
            jsonString += lookAt.y + ",";
            jsonString += lookAt.z + ",";

            jsonString += cameraUniversal.minimumX + ",";
            jsonString += cameraUniversal.maximumX + ",";

            jsonString += cameraUniversal.minimumZ + ",";
            jsonString += cameraUniversal.maximumZ + ",";
            jsonString += cameraUniversal.zhiBeiZhenCorrect;

            jsonString += "]}";

            if (i != cameraUniversalCenter.cameras.Count - 1)
            {
                jsonString += ",";
            }
        }

        jsonString += "}}";

        return jsonString;
    }

    string GetCollisonMeshJson(Transform pzMesh)
    {
        List<float> verticesList = new List<float>();
        Vector3[] vertices;
        int[] index;

        if (pzMesh.GetComponent<MeshFilter>() == null || pzMesh.GetComponent<MeshFilter>().sharedMesh == null)
        {
            string log = pzMesh.name + "碰撞物体没有Mesh！";
            GlobalDebug.Addline(log);
            Debug.Log(log);
            Debug.LogWarning(log);
            Debug.LogError(log);
            return "[]";
        }

        vertices = pzMesh.GetComponent<MeshFilter>().sharedMesh.vertices;
        index = pzMesh.GetComponent<MeshFilter>().sharedMesh.triangles;


        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = pzMesh.TransformPoint(vertices[i]);
        }


        for (int i = 0; i < index.Length / 3; i++)
        {
            verticesList.Add(vertices[index[i * 3 + 2]].x);
            verticesList.Add(vertices[index[i * 3 + 2]].y);
            verticesList.Add(-vertices[index[i * 3 + 2]].z);

            verticesList.Add(vertices[index[i * 3 + 1]].x);
            verticesList.Add(vertices[index[i * 3 + 1]].y);
            verticesList.Add(-vertices[index[i * 3 + 1]].z);

            verticesList.Add(vertices[index[i * 3]].x);
            verticesList.Add(vertices[index[i * 3]].y);
            verticesList.Add(-vertices[index[i * 3]].z);
        }

        string jsonString = "[";
        for (int i = 0; i < verticesList.Count; i++)
        {
            jsonString += verticesList[i];
            if(i!=verticesList.Count-1)
            jsonString += ",";
        }
         jsonString += "]";


        return jsonString;
    }

    string RecordInteractiveActionInfo(InteractiveAction inAct)
    {

        if (inAct.needDisplayRoot != null&&inAct.needDisplayRoot.GetComponent<DontExport>() != null)
        {
            string log = inAct.needDisplayRoot.name + "被用在needDisplayRoot,且又含有DontExport";
            GlobalDebug.Addline(log);
            Debug.Log(log);
            Debug.LogWarning(log);
            Debug.LogError(log);

            return "";
        }


        string jsonString = "{";

        //取得自定义的objectid
        jsonString += "\"gameObjectName\":\"" + (inAct.needDisplayRoot != null ? GetUTF16_2( inAct.needDisplayRoot.name) : "") + "\"";
        jsonString += ",\"needDisplayRoot\":" + (inAct.needDisplayRoot != null ? inAct.needDisplayRoot.GetComponent<Object3DInfo>().objectId:-1);

        jsonString += ",\"needSetCameraName\":\"" + (inAct.cameraUniversal!=null?inAct.cameraUniversal.name+"\"":"\"");

        if (inAct.cameraUniversal != null)
        {


            string[] splitString = inAct.cameraStates.Split(',');
            if (splitString.Length != 6)
            {
                string log = inAct.ToString() + "CameraStates格式设置有错误！";
                GlobalDebug.Addline(log);
                Debug.Log(log);
                Debug.LogWarning(log);
                Debug.LogError(log);
                return "";
            }

            if (splitString[2] != "")
            {
                float zFloatTemp = float.Parse(splitString[2]);
                zFloatTemp = -zFloatTemp;
                splitString[2] = zFloatTemp.ToString();
            }
                
//            if (splitString[5] != "")
//                splitString[5] = "-" + splitString[5];

            jsonString += ",\"cameraStates\":[";
            jsonString += "\""+splitString[0] + "\",";
            jsonString += "\""+splitString[1] + "\",";
            jsonString += "\"" + splitString[2] + "\",";
            jsonString += "\"" + splitString[3] + "\",";
            jsonString += "\"" + splitString[4] + "\",";
            jsonString += "\"" + splitString[5] + "\"]";
        }
        else
        {
            jsonString += ",\"cameraStates\":[]" ;
        }

        if (inAct.pzMesh != null)
        {
            jsonString += ",\"pzMesh\":" + GetCollisonMeshJson(inAct.pzMesh);
        }

        jsonString += "}";

        return jsonString;
    }

    public static string GetUTF16_2(string inStr)
    {


        string outStr = "";

        char[] inStrAllChar = inStr.ToCharArray();

        //      Debug.Log(inStrAllChar.Length);

        for (int i = 0; i < inStrAllChar.Length; i++)
        {
            //            Debug.Log(inStrAllChar[i]);

            if (Regex.IsMatch(inStrAllChar[i].ToString(), @"[\u4e00-\u9fa5]"))
            {
                outStr += "\\u";

                byte[] labelTextBytes = Encoding.BigEndianUnicode.GetBytes(inStrAllChar, i, 1);

                //                Debug.Log(labelTextBytes.Length);

                foreach (byte b in labelTextBytes)
                {
                    outStr += b.ToString("x2");
                }
            }
            else
            {
                outStr += inStrAllChar[i];
            }
        }
        return outStr;
    }


}







