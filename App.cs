using StereoKit;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace StereoKitApp
{
	public class App
	{
		public SKSettings Settings => new SKSettings { 
			appName           = "StereoKit Template",
			assetsFolder      = "Assets",
			displayPreference = DisplayMode.MixedReality
		};

		Pose  cubePose = new Pose(0, 0, -0.5f, Quat.Identity);
		Model cube;
		Matrix4x4 floorTransform = Matrix.TS(new Vector3(0, -1.5f, 0), new Vector3(30, 0.1f, 30));
		Material  floorMaterial;

		// Menu parameters
		Pose ObjectWindowPose = new Pose(0f, 0f, 0f, Quat.Identity);
		// Main Menu
		bool isMainMenu = false;
		bool isPlaceOb = false;
		bool isImportSc = false;
		bool isExportSc = false;
		// Mesh Controller
		bool isMeshSelected = false;
		bool meshValueChanged = false;
		int selectedMeshIndex;
		bool beingEdited = false;
		bool moreOptions = false;
		bool renameMeshOption = false;
		string renameText;
		String Lx, Ly, Lz;
		String Rx, Ry, Rz;
		String Sx, Sy, Sz;
		String dm, depth, subDiv, selectedHandle;
		Pose MeshSelectorWindowPose = new Pose(0f, 0f, 0f, Quat.Identity);

		// Mesh Lists
		List<Mesh> meshList = new List<Mesh>();
		List<Pose> meshPoses = new List<Pose>();
		List<string> meshHandles = new List<string>();
		List<int> meshType = new List<int>();
		int meshCount = 0;

		// Types of meshes
		enum MeshTypeIndex
        {
			Cube = 1,
			Sphere = 2,
			Cylinder = 3,
			Plane = 4,
        }
		string[] meshNames = { "None", "Cube", "Sphere", "Cylinder", "Plane" }; // None because type index starts at 1

		public void Init()
		{
			// Create assets used by the app
			cube = Model.FromMesh(
				Mesh.GenerateRoundedCube(Vec3.One * 0.1f, 0.02f),
				Default.MaterialUI);

			floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
			floorMaterial.Transparency = Transparency.Blend;

			// setting default values
			isMainMenu = true;
			meshCount = 0;
		}//Init

		public void Step()
		{
			if (SK.System.displayType == Display.Opaque)
				Default.MeshCube.Draw(floorMaterial, floorTransform);

			UI.Handle("Cube", ref cubePose, cube.Bounds);
			cube.Draw(cubePose.ToMatrix());

			// Menu
			UIDisplay();

			// Meshes
			RenderMeshes();
			MeshUIs();

            // Mesh Manipulation
            if (isMeshSelected)
            {
				MeshManipulationUI();
            }
            
		}//Step

		// fn to reset all UI vars to false
		void resetMainUIParams()
        {
			isMainMenu = false;
			isPlaceOb = false;
			isImportSc = false;
			isExportSc = false;
        }//resetMainUIParams

		void UIDisplay()
        {
            #region MainMenu
            // Object inserter UI
            if (isMainMenu)
			{
				UI.WindowBegin("Main Menu", ref ObjectWindowPose, new Vector2(24, 0) * U.cm);
				{
					if (UI.Button("Place Object"))
					{
						resetMainUIParams();
						isPlaceOb = true;
					};
					if (UI.Button("Import Scene"))
					{
						resetMainUIParams();
						isImportSc = true;
					};
					if (UI.Button("Export Scene"))
					{
						resetMainUIParams();
						isExportSc = true;
					};
					if (UI.Button("Exit"))
					{
						SK.Quit();
					}
				}
				UI.WindowEnd();
			}
            #endregion
            if (isPlaceOb)
            {
				UI.WindowBegin("Place", ref ObjectWindowPose, new Vector2(24, 0) * U.cm);
                if (UI.Button("Cube"))
                {
					Mesh m = MeshGenerator((int)MeshTypeIndex.Cube, cubeSize: Vec3.One) ;
					meshType.Add((int)MeshTypeIndex.Cube);
					AddMesh(m);
				}
                if (UI.Button("Sphere"))
                {
					Mesh m = MeshGenerator((int)MeshTypeIndex.Sphere, spDiameter: 1.0f);
					meshType.Add((int)MeshTypeIndex.Sphere);
					AddMesh(m);
				}
                if (UI.Button("Cylinder"))
                {
					Mesh m = MeshGenerator((int)MeshTypeIndex.Cylinder, cyDiameter: 1.0f, cyDepth: 1.0f);
					meshType.Add((int)MeshTypeIndex.Cylinder);
					AddMesh(m);
				}
                if (UI.Button("Plane"))
                {
					Mesh m = MeshGenerator((int)MeshTypeIndex.Plane, plSize: Vec2.One);
					meshType.Add((int)MeshTypeIndex.Plane);
					AddMesh(m);
				}
				UI.WindowEnd();
			}
        }//UIDisplay

		Mesh MeshGenerator(int ch, Vec3 cubeSize = default(Vec3), float spDiameter = 1.0f, 
			float cyDiameter = 1.0f, float cyDepth = 1.0f, Vec2 plSize = default(Vec2), int  cySubDiv = 16, int cuSubDiv = 0, int spSubDiv = 4, int plSubDiv = 0) {
			Mesh m = new Mesh();
			switch (ch) {
				case (int)MeshTypeIndex.Cube:
					m = Mesh.GenerateCube(cubeSize, subdivisions: cuSubDiv);
					break;
				case (int)MeshTypeIndex.Sphere:
					m = Mesh.GenerateSphere(spDiameter, subdivisions: spSubDiv);
					break;
				case (int)MeshTypeIndex.Cylinder:
					m = Mesh.GenerateCylinder(cyDiameter, cyDepth, Vec3.Up, subdivisions: cySubDiv);
					break;
				case (int)MeshTypeIndex.Plane:
					m = Mesh.GeneratePlane(plSize, subdivisions: plSubDiv);
					break;
			}
			return m;
		}

		void AddMesh(Mesh m)
        {
			meshList.Add(m);
			Pose mPose = new Pose(0, 0, -0.5f, Quat.Identity);
			meshPoses.Add(mPose);
			string mHandle = meshNames[meshType[meshType.Count-1]] + "_ObNo_" +meshCount.ToString();
			meshHandles.Add(mHandle);
			meshCount++;
		}//AddMesh

		void RenderMeshes()
        {
			for(int i = 0; i < meshList.Count; i++)
            {
				Matrix meshTransform = meshPoses[i].ToMatrix();
				Mesh m = meshList[i];
				m.Draw(Default.Material, meshTransform);
            }
        }//RenderMeshes

		void MeshUIs()
        {
			for(int i = 0; i < meshList.Count; i++)
            {
				Pose meshPose = meshPoses[i];
				if(UI.Handle(meshHandles[i], ref meshPose, meshList[i].Bounds))
                {
					isMeshSelected = true;
					meshValueChanged = true;
					selectedMeshIndex = i;
					meshPoses[i] = meshPose;
				}
            }
        }//MeshUIs

		void MeshManipulationUI()
        {
            if (meshValueChanged) {
				string[] val = GetMeshTransform();
				Lx = val[0];
				Ly = val[1];
				Lz = val[2];
				Rx = val[3];
				Ry = val[4];
				Rz = val[5];
				Sx = val[6];
				Sy = val[7];
				Sz = val[8];

				val = getMeshDetails();
				dm = val[0];
				depth = val[1];
				subDiv = val[2];
				selectedHandle = val[3];

			}
			meshValueChanged = false;
			string[] values = { Lx, Ly, Lz, Rx, Ry, Rz, Sx, Sy, Sz };
			string[] detailValues = { dm, depth, subDiv, Sx, Sy, Sz };

			UI.WindowBegin("Mesh Manipulation", ref MeshSelectorWindowPose, new Vector2(50, 0) * U.cm);

			UI.Label("Name:");
			UI.SameLine();
			UI.Label(selectedHandle);
			UI.Label("Location: ");
			UI.SameLine();
			UI.Label("X=");
			UI.SameLine();
			if (UI.Input("Lx", ref Lx, new Vec2(0.10f, 0f))) MeshTransformBeingEdited(values);
			UI.SameLine();
			UI.Label("Y=");
			UI.SameLine();
			if (UI.Input("Ly", ref Ly, new Vec2(0.10f, 0f))) MeshTransformBeingEdited(values);
			UI.SameLine();
			UI.Label("Z=");
			UI.SameLine();
			if (UI.Input("Lz", ref Lz, new Vec2(0.1f,0f))) MeshTransformBeingEdited(values);

			UI.Label("Rotation: ");
			UI.SameLine();
			UI.Label("X=");
			UI.SameLine();
			if (UI.Input("Rx", ref Rx, new Vec2(0.10f, 0f))) MeshTransformBeingEdited(values);
			UI.SameLine();
			UI.Label("Y=");
			UI.SameLine();
			if (UI.Input("Ry", ref Ry, new Vec2(0.10f, 0f))) MeshTransformBeingEdited(values);
			UI.SameLine();
			UI.Label("Z=");
			UI.SameLine();
			if (UI.Input("Rz", ref Rz, new Vec2(0.10f, 0f))) MeshTransformBeingEdited(values);

			UI.Label("Scale: ");
			UI.SameLine();

            switch (meshType[selectedMeshIndex])
            {
				case (int)MeshTypeIndex.Cube:
					UI.Label("X=");
					UI.SameLine();
					if (UI.Input("Sx", ref Sx, new Vec2(0.10f, 0f))) MeshScaleChanged(detailValues);
					UI.SameLine();
					UI.Label("Y=");
					UI.SameLine();
					if (UI.Input("Sy", ref Sy, new Vec2(0.10f, 0f))) MeshScaleChanged(detailValues);
					UI.SameLine();
					UI.Label("Z=");
					UI.SameLine();
					if (UI.Input("Sz", ref Sz, new Vec2(0.10f, 0f))) MeshScaleChanged(detailValues);
					break;
				case (int)MeshTypeIndex.Sphere:
					UI.Label("Diameter=");
					UI.SameLine();
					if (UI.Input("Dm", ref dm, new Vec2(0.10f, 0f))) MeshScaleChanged(detailValues);
					break;
				case (int)MeshTypeIndex.Cylinder:
					UI.Label("Diameter=");
					UI.SameLine();
					if (UI.Input("Dm", ref dm, new Vec2(0.10f, 0f))) MeshScaleChanged(detailValues);
					break;
				case (int)MeshTypeIndex.Plane:
					UI.Label("X=");
					UI.SameLine();
					if (UI.Input("Sx", ref Sx, new Vec2(0.10f, 0f))) MeshScaleChanged(detailValues);
					break;
            }
			
			

            if (!moreOptions)
            {
				if (UI.Button("More Options")) moreOptions = true;
			}
            if (moreOptions)
            {
				if(UI.Button("Delete Mesh"))
                {
					MeshDeleted();
                }
				UI.SameLine();
				if (UI.Button("Rename Mesh"))
                {
					renameMeshOption = true;
					renameText = meshHandles[selectedMeshIndex];
				}
                if (renameMeshOption)
                {
					UI.Label("Name: ");
					UI.SameLine();
					UI.Input("Rename", ref renameText, new Vec2(0.3f, 0));
					UI.SameLine();
                    if (UI.Button("Done"))
                    {
						RenameMesh();
                    }
                }
				if (UI.Button("Less Options")) moreOptions = false;
            }
			UI.WindowEnd();
        }

		string[] GetMeshTransform()
        {
			Pose selectedMeshPose = meshPoses[selectedMeshIndex];
			Matrix transform = selectedMeshPose.ToMatrix();
			string lx = transform.Translation.x.ToString();
			string ly = transform.Translation.y.ToString();
			string lz = transform.Translation.z.ToString();
			string rx = transform.Rotation.x.ToString();
			string ry = transform.Rotation.y.ToString();
			string rz = transform.Rotation.z.ToString();
			string sx = transform.Scale.x.ToString();
			string sy = transform.Scale.y.ToString();
			string sz = transform.Scale.z.ToString();
			string[] ret = { lx, ly, lz, rx, ry, rz, sx, sy, sz };
			return ret;
		}

		void SetMeshTransform(string[] val)
        {
            try
            {
				float lx = float.Parse(val[0], CultureInfo.InvariantCulture.NumberFormat);
				float ly = float.Parse(val[1], CultureInfo.InvariantCulture.NumberFormat);
				float lz = float.Parse(val[2], CultureInfo.InvariantCulture.NumberFormat);

				float rx = float.Parse(val[3], CultureInfo.InvariantCulture.NumberFormat);
				float ry = float.Parse(val[4], CultureInfo.InvariantCulture.NumberFormat);
				float rz = float.Parse(val[5], CultureInfo.InvariantCulture.NumberFormat);

				float sx = float.Parse(val[6], CultureInfo.InvariantCulture.NumberFormat);
				float sy = float.Parse(val[7], CultureInfo.InvariantCulture.NumberFormat);
				float sz = float.Parse(val[8], CultureInfo.InvariantCulture.NumberFormat);

				Quaternion q = Quat.FromAngles(new Vec3(rx, ry, rz));
				meshPoses[selectedMeshIndex] = new Pose(lx, ly, lz, q);
			}
            catch
            {
				// Nothing here
				/* Error will be thrown when user clears value in the input field and we catch that */
            }
			
		}//SetMeshTransform

		void MeshTransformBeingEdited(string[] val)
        {
			SetMeshTransform(val);
        }//MeshTransfromBeingEdited

		void MeshScaleChanged(string[] val)
        {
            try
            {
				float diameter = float.Parse(val[0], CultureInfo.InvariantCulture.NumberFormat);
				float depth = float.Parse(val[1], CultureInfo.InvariantCulture.NumberFormat);
				int subDivisions = int.Parse(val[2], CultureInfo.InvariantCulture.NumberFormat);
				float sx = float.Parse(val[3], CultureInfo.InvariantCulture.NumberFormat);
				float sy = float.Parse(val[4], CultureInfo.InvariantCulture.NumberFormat);
				float sz = float.Parse(val[5], CultureInfo.InvariantCulture.NumberFormat);

				meshList[selectedMeshIndex] = MeshGenerator(meshType[selectedMeshIndex], cubeSize: new Vec3(sx, sy, sz), 
					spDiameter: diameter, cyDiameter: diameter, cyDepth: depth, plSize: new Vec2(sx, sz), 
					cySubDiv: subDivisions, cuSubDiv: subDivisions, spSubDiv: subDivisions, plSubDiv: subDivisions);
			}
            catch
            {
				// error thrown when the input fields are empty
            }
        }

		void MeshDeleted()
        {
			isMeshSelected = false;
			meshList.RemoveAt(selectedMeshIndex);
			meshHandles.RemoveAt(selectedMeshIndex);
			meshPoses.RemoveAt(selectedMeshIndex);
			meshType.RemoveAt(selectedMeshIndex);
			meshCount--;
        }//MeshDeleted

		void RenameMesh()
        {
			meshHandles[selectedMeshIndex] = renameText;
			renameMeshOption = false;
        }//RenameMesh

		void DuplicateMesh()
        {
			int type = meshType[selectedMeshIndex];
			float diameter = meshList[selectedMeshIndex].Bounds.dimensions.x;
			float depth = meshList[selectedMeshIndex].Bounds.dimensions.z;
			int subDiv = meshList[selectedMeshIndex].GetVerts().Length / 4;
			MeshGenerator(type, spDiameter: diameter, cyDiameter: diameter, cyDepth: depth, plSize: new Vec2(diameter, diameter), cySubDiv: subDiv, cuSubDiv: subDiv, spSubDiv: subDiv, plSubDiv: subDiv);
        }//DuplicateMesh

		string[] getMeshDetails()
        {
			int type = meshType[selectedMeshIndex];
			string diameter = meshList[selectedMeshIndex].Bounds.dimensions.x.ToString();
			string depth = meshList[selectedMeshIndex].Bounds.dimensions.z.ToString();
			string subDiv = (meshList[selectedMeshIndex].GetVerts().Length / 4).ToString();
			string handle = meshHandles[selectedMeshIndex];
			string[] arr = { diameter, depth, subDiv, handle};
			return arr;
		}
	}
}