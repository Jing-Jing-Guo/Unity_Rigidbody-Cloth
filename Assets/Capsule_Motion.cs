 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule_name{

public class Capsule_Motion : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody capsule;

    private CapsuleCollider capsule_collider;
    float mass;
    public MeshFilter cloth_mesh;
    private float scale; 
    private float height;
    private float radius;
    public static ArrayList nodes;

    public static bool capsule_flag;
    void Start()
    {
        
        capsule = GetComponent<Rigidbody>();
        capsule_collider = GetComponent<CapsuleCollider>();
        mass = capsule.mass;
        scale = capsule_collider.transform.localScale.x;
        radius = capsule_collider.radius * scale;
        height = (capsule_collider.height - radius * 2) * scale;
        capsule_flag = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 G_force = new Vector3(0, -9.8f * mass, 0);
        //capsule.AddForce(force);


        Vector3 position = transform.position;
        GameObject cloth = GameObject.Find("cloth");
        cloth_mesh = cloth.GetComponent<MeshFilter>();

        Vector3 mesh_position = cloth.transform.position;
        //Vector3 pos_former = position;

        Vector3[] Y = cloth_mesh.mesh.vertices;

        for(int i = 0; i < Y.Length; i ++){
            Y[i] += mesh_position;
        }

        Vector3[] normals = cloth_mesh.mesh.normals;

        Vector3 force = G_force;

        print("capsule_flag:");
        print(capsule_flag);

        if(capsule_flag){
            int[] nodes_int = (int[]) nodes.ToArray(typeof(int));

            for(int j = 0; j < nodes_int.Length; j ++){
                force += Vector3.Dot(-G_force, normals[nodes_int[j]]) * normals[nodes_int[j]];
            }
        }
/*

        for(int j = 0; j < Y.Length; j ++){
            float distance = (position - Y[j]).magnitude;

            float distance_y = position.y - Y[j].y;
            float distance_x = position.x - Y[j].x;
            float distance_z = position.z - Y[j].z;

            if(Mathf.Abs(distance_y) <= scale.y && distance <= scale.y){
                Vector3 ver_force_y = new Vector3(0, (9.8f * mass  + 3f) * direction(distance_y), 0);
                //print(distance_y);
                capsule.AddForce(ver_force_y);
            }

            if(Mathf.Abs(distance_x) <= scale.x && distance < scale.x){
                Vector3 ver_force_x = new Vector3(1f * direction(distance_x), 0, 0);
                //print(distance_y);
                capsule.AddForce(ver_force_x);
            }

            if(Mathf.Abs(distance_z) <= scale.z && distance < scale.z){
                Vector3 ver_force_z = new Vector3(0, 0, 1f * direction(distance_z));
                //print(distance_y);
                capsule.AddForce(ver_force_z);
            }
/*

            if(distance < 1f){
                
                
                print(position);
                //capsule.AddForce(ver_force);
                //print(distance);
                }
                */
            //if((pos_former.z - position.z) > 0.01f){
            //    cube.AddForce(error);
            //}
        print("capsule_force");
        print(force);

        capsule.AddForce(force);
        set_Capsule_Flag(false);

    }

    public bool get_Capsule_Flag(){
        return capsule_flag;
    }

    public static void set_Capsule_Flag(bool a){
        capsule_flag = a;
    }

    public static void set_Capsule_Nodes(ArrayList a){
        nodes = a;
    }

    int direction(float x){
        if(x < 0){
            return -1;
        }
        else{
            return 1;
        }
    }
}
}