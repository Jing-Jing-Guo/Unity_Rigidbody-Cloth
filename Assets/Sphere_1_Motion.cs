 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere_1_Motion : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody sphere_1;
    float mass;
    float t = 0.0333f;
    public MeshFilter cloth_mesh;
    void Start()
    {
        
        sphere_1 = GetComponent<Rigidbody>();
        mass = sphere_1.mass;
        //Vector3 force = new Vector3(0, 9.8f * mass, 0);
        //sphere_1.AddForce(force);
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 force = new Vector3(0, -9.8f * mass, 0);
        sphere_1.AddForce(force);


        Vector3 position = transform.position;
        Vector3 scale = transform.localScale;
        //Vector3 pos_former = position;

        Vector3[] Y = cloth_mesh.mesh.vertices;

        for(int j = 0; j < Y.Length; j ++){
            float distance = (position - Y[j]).magnitude;

            float distance_y = position.y - Y[j].y;
            float distance_x = position.x - Y[j].x;
            float distance_z = position.z - Y[j].z;

            if(Mathf.Abs(distance_y) <= scale.y && distance <= scale.y){
                Vector3 ver_force_y = new Vector3(0, (9.8f * mass) * direction(distance_y), 0);
                //print(distance_y);
                sphere_1.AddForce(ver_force_y);
            }

            if(Mathf.Abs(distance_x) <= scale.x && distance < scale.x){
                Vector3 ver_force_x = new Vector3(0.1f * direction(distance_x), 0, 0);
                //print(distance_y);
                sphere_1.AddForce(ver_force_x);
            }

            if(Mathf.Abs(distance_z) <= scale.z && distance < scale.z){
                Vector3 ver_force_z = new Vector3(0, 0, 0.1f * direction(distance_z));
                //print(distance_y);
                sphere_1.AddForce(ver_force_z);
            }

            }

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
