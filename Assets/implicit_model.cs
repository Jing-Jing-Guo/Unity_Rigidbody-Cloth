using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Cube_name;
using Capsule_name;
public class implicit_model : MonoBehaviour
{
	float 		t 		= 0.0333f;
	float 		mass	= 1;
	float		damping	= 0.99f;
	float 		rho		= 0.995f;
	float 		spring_k = 8000;
	int[] 		E;
	float[] 	L;
	Vector3[] 	V;

	

    // Start is called before the first frame update
    void Start()
    {
		Mesh mesh = GetComponent<MeshFilter> ().mesh;

		//Resize the mesh.
		int n=21;
		Vector3[] X  	= new Vector3[n*n];
		Vector2[] UV 	= new Vector2[n*n];
		int[] triangles	= new int[(n-1)*(n-1)*6];
		for(int j=0; j<n; j++)
		for(int i=0; i<n; i++)
		{
			X[j*n+i] =new Vector3(5-10.0f*i/(n-1), 0, 5-10.0f*j/(n-1));
			UV[j*n+i]=new Vector3(i/(n-1.0f), j/(n-1.0f));
		}
		int t=0;
		for(int j=0; j<n-1; j++)
		for(int i=0; i<n-1; i++)	
		{
			triangles[t*6+0]=j*n+i;
			triangles[t*6+1]=j*n+i+1;
			triangles[t*6+2]=(j+1)*n+i+1;
			triangles[t*6+3]=j*n+i;
			triangles[t*6+4]=(j+1)*n+i+1;
			triangles[t*6+5]=(j+1)*n+i;
			t++;
		}
		mesh.vertices=X;
		mesh.triangles=triangles;
		mesh.uv = UV;
		mesh.RecalculateNormals ();


		//Construct the original E
		int[] _E = new int[triangles.Length*2];
		for (int i=0; i<triangles.Length; i+=3) 
		{
			_E[i*2+0]=triangles[i+0];
			_E[i*2+1]=triangles[i+1];
			_E[i*2+2]=triangles[i+1];
			_E[i*2+3]=triangles[i+2];
			_E[i*2+4]=triangles[i+2];
			_E[i*2+5]=triangles[i+0];
		}
		//Reorder the original edge list
		for (int i=0; i<_E.Length; i+=2)
			if(_E[i] > _E[i + 1]) 
				Swap(ref _E[i], ref _E[i+1]);
		//Sort the original edge list using quicksort
		Quick_Sort (ref _E, 0, _E.Length/2-1);

		//移除重复的边
		int e_number = 0;
		for (int i=0; i<_E.Length; i+=2)
			if (i == 0 || _E [i + 0] != _E [i - 2] || _E [i + 1] != _E [i - 1]) 
					e_number++;

		//_E 原始的边的顶点坐标

		//E 非重合的边的顶点的ID
		//所以边的数量为E.Length/2
		E = new int[e_number * 2];
		for (int i=0, e=0; i<_E.Length; i+=2)
			if (i == 0 || _E [i + 0] != _E [i - 2] || _E [i + 1] != _E [i - 1]) 
			{
				E[e*2+0]=_E [i + 0];
				E[e*2+1]=_E [i + 1];
				e++;
			}

		//L 存边，边的长度
		L = new float[E.Length/2];
		for (int e=0; e<E.Length/2; e++) 
		{
			int v0 = E[e*2+0];
			int v1 = E[e*2+1];
			L[e]=(X[v0]-X[v1]).magnitude;
		}

		V = new Vector3[X.Length];
		for (int i=0; i<V.Length; i++)
			V[i] = new Vector3 (0, 0, 0);
    }

    void Quick_Sort(ref int[] a, int l, int r)
	{
		int j;
		if(l<r)
		{
			j=Quick_Sort_Partition(ref a, l, r);
			Quick_Sort (ref a, l, j-1);
			Quick_Sort (ref a, j+1, r);
		}
	}

	int  Quick_Sort_Partition(ref int[] a, int l, int r)
	{
		int pivot_0, pivot_1, i, j;
		pivot_0 = a [l * 2 + 0];
		pivot_1 = a [l * 2 + 1];
		i = l;
		j = r + 1;
		while (true) 
		{
			do ++i; while( i<=r && (a[i*2]<pivot_0 || a[i*2]==pivot_0 && a[i*2+1]<=pivot_1));
			do --j; while(  a[j*2]>pivot_0 || a[j*2]==pivot_0 && a[j*2+1]> pivot_1);
			if(i>=j)	break;
			Swap(ref a[i*2], ref a[j*2]);
			Swap(ref a[i*2+1], ref a[j*2+1]);
		}
		Swap (ref a [l * 2 + 0], ref a [j * 2 + 0]);
		Swap (ref a [l * 2 + 1], ref a [j * 2 + 1]);
		return j;
	}

	void Swap(ref int a, ref int b)
	{
		int temp = a;
		a = b;
		b = temp;
	}

	void Collision_Handling()
	{
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		Vector3[] X = mesh.vertices;
		
		//Handle colllision.
		//sphere
		GameObject sphere = GameObject.Find("Sphere");
		Vector3 c = sphere.transform.position;
		float r = 2.7f;

		GameObject cube = GameObject.Find("Cube");
		Vector3 o = cube.transform.position;
		float a = 1f;

		GameObject capsule = GameObject.Find("Capsule");
		Vector3 o_1 = capsule.transform.position;
		float a_1 = 1f;

		GameObject sphere_1 = GameObject.Find("Sphere_1");
		Vector3 c_1 = sphere_1.transform.position;
		float r_1 = 1f;


		//Box box = new Box();
		Collider box_collider;
		box_collider = cube.GetComponent<Collider>();
		//box = get_Box(box_collider, box);
		Vector3 box_size = box_collider.bounds.size;
		float box_scale = box_collider.transform.localScale.x;


		CapsuleCollider capsule_collider;
		capsule_collider = capsule.GetComponent<CapsuleCollider>();
		Vector3 capsule_size = capsule_collider.bounds.size;
		float capsule_scale = capsule_collider.transform.localScale.x;
		float capsule_radius = capsule_collider.radius * capsule_scale;
		float capsule_height = (capsule_collider.height - capsule_radius * 2f) * capsule_scale;



		ArrayList cube_nodes = new ArrayList();
		ArrayList capsule_nodes = new ArrayList();

		
		

		for(int i = 0; i < X.Length; i ++){
			if(i == 0 || i == 20 || i == 420 || i == 440)
				continue;
			Vector3 xi = X[i];
			//print(xi);
			Vector3 vi = V[i];

			float distance = (xi - c).magnitude;
			
			if(distance <= r){
				V[i] = vi + (c + r * (xi - c) / (xi - c).magnitude - xi) / t;
				X[i] = c + r * (xi - c) / (xi - c).magnitude;
			}
			
			float box_sdf = SDF_Box(X[i], o, box_size);
			if(box_sdf - 0.3f * box_size.x <= 0.0f){
				V[i] += -(box_sdf - 0.3f * box_size.x) * Gradient_Box_SDF(X[i], o, box_size * box_scale, box_sdf - 0.3f * box_size.x) / t;
				X[i] += -(box_sdf - 0.3f * box_size.x) * Gradient_Box_SDF(X[i], o, box_size * box_scale, box_sdf - 0.3f * box_size.x);
				Cube_Motion.set_Box_Flag(true);
				cube_nodes.Add(i);
			}

			float capsule_sdf = SDF_Capsule(X[i], o_1, capsule_radius, capsule_height);
			if(capsule_sdf - 0.4f * capsule_scale <= 0){
				V[i] += -(capsule_sdf - 0.3f * capsule_scale) * Gradient_Capsule_SDF(X[i], o_1, capsule_radius, capsule_height) / t;
				X[i] += -(capsule_sdf - 0.3f * capsule_scale) * Gradient_Capsule_SDF(X[i], o_1, capsule_radius, capsule_height);
				Capsule_Motion.set_Capsule_Flag(true);
				capsule_nodes.Add(i);
			}

/*

			float distance_cube = (xi - o).magnitude;

			if(distance_cube <= box_size.x){
				V[i] = vi + (o + a * (xi - o) / (xi - o).magnitude - xi) / t;
				//X[i] = o + a * (xi - o) / (xi - o).magnitude;
				X[i] = o + box_size.x * (xi - o - box_size) / (xi - o- box_size).magnitude;
			}
*/

/*
			float distance_capsule = (xi - o_1).magnitude;

			if(distance_capsule <= a_1){
				V[i] = vi + (o_1 + a_1 * (xi - o_1) / (xi - o_1).magnitude - xi) / t;
				X[i] = o_1 + a_1 * (xi - o_1) / (xi - o_1).magnitude;
			}

			float distance_sphere_1 = (xi - c_1).magnitude;
			
			if(distance_sphere_1 <= r_1){
				V[i] = vi + (c_1 + r_1 * (xi - c_1) / (xi - c_1).magnitude - xi) / t;
				X[i] = c_1 + r_1 * (xi - c_1) / (xi - c_1).magnitude;
			}
*/

			

			
		}

		Cube_Motion.set_nodes(cube_nodes);
		Capsule_Motion.set_Capsule_Nodes(capsule_nodes);



		mesh.vertices = X;
	}

	void Get_Gradient(Vector3[] X, Vector3[] X_hat, float t, Vector3[] G)
	{
		//Momentum and Gravity.
		for(int i = 0; i < X.Length; i ++){
			Vector3 xi = X[i];
			Vector3 xi_hat = X_hat[i];

			//未考虑弹簧
			G[i] = (xi - xi_hat) * mass / (t * t);
		}

		//逐边计算
		for(int k = 0; k <L.Length; k++){
			//每条边的长度
			float le = L[k];
			int v0 = E[k*2 + 0];
			int v1 = E[k*2 + 1];
			G[v0] += spring_k * (1 - le / (X[v0] - X[v1]).magnitude) * (X[v0] - X[v1]);
			G[v1] -= spring_k * (1 - le / (X[v0] - X[v1]).magnitude) * (X[v0] - X[v1]);

		}

		//考虑重力
		for(int i = 0; i < X.Length; i ++){
			G[i] -= Vector3.down * 9.8f * mass;
		}


		//Spring Force.
		
	}

	float SDF_Box(Vector3 point, Vector3 position, Vector3 size){
		float Rx = size.x * 0.5f;
		float Ry = size.y * 0.5f;
		float Rz = size.z * 0.5f;

		Vector3 relative_point = point - position;
		
		Vector3 distance = new Vector3(
			math.max(math.abs(relative_point.x) - Rx, 0),
			math.max(math.abs(relative_point.y) - Ry, 0),
			math.max(math.abs(relative_point.z) - Rz, 0)
		);
		float sdf = distance.magnitude + math.min((math.max(math.abs(relative_point.x) - Rx, math.max(math.abs(relative_point.y) - Ry, math.abs(relative_point.z) - Rz))), 0);
		return sdf;
	}

	Vector3 Gradient_Box_SDF(Vector3 point, Vector3 position, Vector3 size, float sdf){

		float Rx = size.x * 0.5f;
		float Ry = size.y * 0.5f;
		float Rz = size.z * 0.5f;

		Vector3 relative_point = point - position;
		//print(relative_point);
		Vector3 distance = new Vector3(
			math.max(math.abs(relative_point.x) - Rx, 0),
			math.max(math.abs(relative_point.y) - Ry, 0),
			math.max(math.abs(relative_point.z) - Rz, 0)
		);

		float x = 0.0f;
		float y = 0.0f;
		float z = 0.0f;
		
		//print("sdf:");
		//print(sdf);
		sdf = distance.magnitude + math.min((math.max(math.abs(relative_point.x) - Rx, math.max(math.abs(relative_point.y) - Ry, math.abs(relative_point.z) - Rz))), 0);

		//print(sdf);
		if(sdf == math.abs(relative_point.x) - Rx){
			//print("x");
			if(relative_point.x >= 0){
				x = 1f;
			}
			else{
				x = -1f;
			}

		}
		else if(sdf == math.abs(relative_point.y) - Ry){
			//print("y");
			//print(relative_point.y);
			if(relative_point.y > 0){
				y = 1f;
			}
			else{
				y = -1f;
			}

		}
		else if(sdf == math.abs(relative_point.z) - Rz){
			//print("z");
			if(relative_point.z >= 0){
				z = 1f;
			}
			else{
				z = -1f;
			}
		}
		else{
			if(relative_point.x >= 0){
				x = Mathf.Sqrt(2) / 2;
			}
			else{
				x = -Mathf.Sqrt(2) / 2;
			}
			if(relative_point.y >= 0){
				y = Mathf.Sqrt(2) / 2;
			}
			else{
				y = -Mathf.Sqrt(2) / 2;
			}
			if(relative_point.z >= 0){
				z = Mathf.Sqrt(2) / 2;
			}
			else{
				z = -Mathf.Sqrt(2) / 2;
			}
		}
		Vector3 current_po =  new Vector3(x, y, z);
		return current_po;
	}


	float SDF_Capsule(Vector3 point, Vector3 position, float radius, float h){
		float Rx = radius;
		float Ry = h * 0.5f;
		float Rz = radius;

		Vector3 relative_point = point - position;

		Vector3 distance = new Vector3(
			math.max(math.abs(relative_point.x) - Rx, 0),
			math.max(math.sqrt(relative_point.x * relative_point.x + (math.abs(relative_point.y) - Ry) * (math.abs(relative_point.y) - Ry)) - Rx, 0),
			math.max(math.abs(relative_point.z) - Rz, 0)
		);

		float sdf = distance.magnitude + math.min(math.max(math.abs(relative_point.x) - Rx, math.max(math.sqrt(relative_point.x * relative_point.x + (math.abs(relative_point.y) - Ry) * (math.abs(relative_point.y) - Ry)) - Rx, math.abs(relative_point.z) - Rz)), 0);
		return sdf;
	}

	Vector3 Gradient_Capsule_SDF(Vector3 point, Vector3 position, float radius, float h){
		float Rx = radius;
		float Ry = h * 0.5f;
		float Rz = radius;

		Vector3 relative_point = point - position;

		Vector3 distance = new Vector3(
			math.max(math.abs(relative_point.x) - Rx, 0),
			math.max(math.sqrt(relative_point.x * relative_point.x + (math.abs(relative_point.y) - Ry) * (math.abs(relative_point.y) - Ry)) - Rx, 0),
			math.max(math.abs(relative_point.z) - Rz, 0)
		);

		float sdf = distance.magnitude + math.min(math.max(math.abs(relative_point.x) - Rx, math.max(math.sqrt(relative_point.x * relative_point.x + (math.abs(relative_point.y) - Ry) * (math.abs(relative_point.y) - Ry)) - Rx, math.abs(relative_point.z) - Rz)), 0);

		float x = 0f;
		float y = 0f;
		float z = 0f;

		Vector3 current_po = new Vector3(0f, 0f, 0f);

		if(sdf == math.abs(relative_point.x) - Rx){
			if(relative_point.x >= 0){
				current_po.x = 1f;
			}
			else{
				current_po.x = -1f;
			}
		}
		else if(sdf == math.abs(relative_point.z) - Rz){
			if(relative_point.z >= 0){
				current_po.z = 1f;
			}
			else{
				current_po.z = -1f;
			}
		}
		else{
			if(relative_point.y >= 0){
				current_po = (point - (position + new Vector3(0f, Ry, 0f))) / (point - (position + new Vector3(0f, Ry, 0f))).magnitude;
			}
			else{
				current_po = (point - (position + new Vector3(0f, -Ry, 0f))) / (point - (position + new Vector3(0f, -Ry, 0f))).magnitude;
			}
		}

		return current_po;
	}



    // Update is called once per frame
	void Update () 
	{
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		Vector3[] X 		= mesh.vertices;
		Vector3[] last_X 	= new Vector3[X.Length];
		Vector3[] X_hat 	= new Vector3[X.Length];
		Vector3[] G 		= new Vector3[X.Length];

		//Initial Setup.
		for(int i = 0; i < X.Length; i ++){
			V[i] *= damping;
			X_hat[i] = t * (Vector3)V[i] + X[i];
		}
		for(int i = 0; i < X.Length; i++){
			X[i] = X_hat[i];
		}


		//迭代，默认迭代32次
		for(int k=0; k<32; k++)
		{
			Get_Gradient(X, X_hat, t, G);
			
			//Update X by gradient.
			for(int i = 0; i < X.Length; i ++){
				if(i == 0 || i == 20 || i == 420 || i == 440)
					//固定钉住
					continue;
				X[i] -= 1 / (mass / (t * t) + 4 * spring_k) * G[i];
			}
			
		}

		//计算速度
		for(int i = 0; i < X.Length; i ++){
			//V[i] += (float3)(X[i] - X_hat[i]) / t;
			V[i] += (X[i] - X_hat[i]) / t;
		}

		//Finishing.
		
		mesh.vertices = X;
		Collision_Handling ();
		mesh.RecalculateNormals ();
	}
}


