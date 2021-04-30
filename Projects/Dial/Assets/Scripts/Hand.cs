using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    float width = 0.1f;

    private GameManager manager;
    private GameObject[] joints = new GameObject[21];
    private GameObject[] bones = new GameObject[15];

    float[,] pos;

    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

       float[,] newPos = new float[21,3] {
            { 4.75001544E-01f,  7.41917133E-01f,  2.21458763E-01f},
            { 1.63009092E-02f,  5.96636355E-01f,  1.43576756E-01f},
            {-2.43968546E-01f,  3.45346123E-01f, -1.57453269E-02f},
            {-4.19878900E-01f,  2.70308316E-01f, -1.70763120E-01f},
            {-5.44546545E-01f,  1.72325909E-01f, -1.31850600E-01f},
            {-1.44571781E-01f,  3.99830565E-03f, -1.13904905E-02f},
            {-2.95212209E-01f, -1.66675627E-01f, -1.57668516E-01f},
            {-3.91344547E-01f, -2.82495260E-01f, -3.43756139E-01f},
            {-4.71050441E-01f, -3.74647021E-01f, -4.88100767E-01f},
            {-5.75919135E-09f,  1.48054191E-08f,  9.24942611E-09f},
            {-1.58388585E-01f, -1.88514888E-01f, -2.10438102E-01f},
            {-2.52010882E-01f, -2.01916307E-01f, -3.97168845E-01f},
            {-2.63900250E-01f, -3.44180048E-01f, -6.63591266E-01f},
            { 1.18877850E-01f, -8.17735493E-03f,  7.16908742E-03f},
            { 3.68870236E-02f, -2.17601717E-01f, -2.07681298E-01f},
            { 1.19584575E-02f, -2.35350102E-01f, -3.87541294E-01f},
            {-3.12373079E-02f, -2.96420366E-01f, -4.91445065E-01f},
            { 3.33521008E-01f,  1.83711573E-02f, -3.93945873E-02f},
            { 2.30161816E-01f, -2.30540156E-01f, -2.25254849E-01f},
            { 1.85321793E-01f, -3.23120236E-01f, -3.84193659E-01f},
            { 1.00466520E-01f, -4.05423403E-01f, -3.68712515E-01f}
        };

        InitializeObjects();
        UpdatePos(newPos);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePos(manager.GetPose());
        UpdateJointsAndBones();
    }

    public void UpdatePos(float[,] newPos){
        pos = newPos;
    }

    void ChangeCylinderBetweenPoints(ref GameObject cylinder, Vector3 start, Vector3 end)
    {
        var offset = end - start;
        var scale = new Vector3(width, offset.magnitude / 2.0f, width);
        var position = start + (offset / 2.0f);
 
        cylinder.transform.position = position;
        cylinder.transform.up = offset;
        cylinder.transform.localScale = scale;
    }

    void InitializeObjects() {
        for (int i = 0; i < 21; i++) {
            joints[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            joints[i].name = "Joint" + i.ToString();
            joints[i].transform.localScale = new Vector3(width, width, width);
            joints[i].transform.parent = this.transform;
        }

        for (int i = 0; i < 15; i++) {
            bones[i] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bones[i].name = "Bone" + i.ToString();
            bones[i].transform.localScale = new Vector3(width, width, width);
            bones[i].transform.parent = this.transform;
        }
    }

    void UpdateJointsAndBones() {
        for (int i = 0; i < 21; i++) {
            joints[i].transform.position = new Vector3(pos[i, 0], pos[i, 1], pos[i, 2]);
        }

        for (int i = 0; i < 5; i++) {
            for(int j = 0; j < 3; j++) {
                ChangeCylinderBetweenPoints(ref bones[i*3+j], new Vector3(pos[i*4+j+1, 0], pos[i*4+j+1, 1], pos[i*4+j+1, 2]), new Vector3(pos[i*4+j+2, 0], pos[i*4+j+2, 1], pos[i*4+j+2, 2]));
            }
        }
    }
}
