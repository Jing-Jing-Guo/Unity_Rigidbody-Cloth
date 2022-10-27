 using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Cube_name{

public class Cube_Motion : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody cube;
    private Collider cube_collider;
    float mass;
    public MeshFilter cloth_mesh;
    private Vector3 size;

    public static ArrayList nodes;

    public static bool cube_flag;
    void Start()
    {
        
        cube = GetComponent<Rigidbody>();
        cube_collider = GetComponent<Collider>();
        mass = cube.mass;
        size = cube_collider.bounds.size;
        cube_flag = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 G_force = new Vector3(0, -9.8f * mass, 0);
        //cube.AddForce(G_force);

        Vector3 position = transform.position;
        GameObject cloth = GameObject.Find("cloth");
        cloth_mesh = cloth.GetComponent<MeshFilter>();

        Vector3 mesh_position = cloth.transform.position;

        Vector3[] Y = cloth_mesh.mesh.vertices;

        for(int i = 0;i < Y.Length; i ++){
            Y[i] += mesh_position;
        }

       // float force_1 = 10f;
        
        //Vector3 ver_force = new Vector3(0, force_1, 0f);

        Vector3[] normals = cloth_mesh.mesh.normals;

        //Vector3 force = new Vector3(0f, 0f, 0f);
        Vector3 force = G_force;
        
        print("cube_flag:");
        print(cube_flag);


        if(cube_flag){
            int[] nodes_int = (int[]) nodes.ToArray(typeof(int));

            for(int j = 0; j < nodes_int.Length;j ++){
                force += Vector3.Dot(-G_force, normals[nodes_int[j]]) * normals[nodes_int[j]];
            }
        }
        
        /*
        for(int j = 0; j < Y.Length; j ++){

/*

            float Rx = size.x * 0.5f;
            float Ry = size.y * 0.5f;
            float Rz = size.z * 0.5f;

            Vector3 relative_point = Y[j] - position;
            Vector3 distance = new Vector3(
                math.max(math.abs(relative_point.x) - Rx, 0),
                math.max(math.abs(relative_point.y) - Ry, 0),
                math.max(math.abs(relative_point.z) - Rz, 0)
            );

            float sdf = distance.magnitude + math.min((math.max(math.abs(relative_point.x) - Rx, math.max(math.abs(relative_point.y) - Ry, math.abs(relative_point.z) - Rz))), 0);


            if(sdf - 0.2f * size.x <= 0.0f){
                print(sdf);
                force += Vector3.Dot(-G_force, normals[j]) * normals[j];
            }
            */
            /*

            if(cube_flag){
                force += Vector3.Dot(-G_force, normals[j]) * normals[j];
            }
            */

/*


            float distance = (position - Y[j]).magnitude;

            float distance_y = position.y - Y[j].y;
            float distance_x = position.x - Y[j].x;
            float distance_z = position.z - Y[j].z;

            //这里的碰撞检测我就简单地使用距离判断，就没有用sdf进行判断
            //if(Mathf.Abs(distance_y) <= size.y && distance <= size.y || Mathf.Abs(distance_x) <= size.x && distance < size.x || Mathf.Abs(distance_z) <= size.z && distance < size.z){
            if(Mathf.Abs(distance_y) <= 0.6f * size.y && distance <= 0.6f * size.y || Mathf.Abs(distance_x) <= 0.6f * size.x && distance < 0.6f * size.x || Mathf.Abs(distance_z) <= 0.6f * size.z && distance < 0.6f * size.z){
                force += Vector3.Dot(-G_force, normals[j]) * normals[j];
            }
            
    }
    */
    print("cube_force:");
    print(force);

    cube.AddForce(force);
    set_Box_Flag(false);
    }

    public bool get_Box_Flag(){
		return cube_flag;
	}

	public static void set_Box_Flag(bool a){
		cube_flag = a;
	}

    public static void set_nodes(ArrayList a){
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