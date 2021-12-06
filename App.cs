using StereoKit;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;

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
		// Place Menu
		bool isPlaceMenu = false;

		// Mesh Lists
		List<Mesh> meshList = new List<Mesh>();
		List<Pose> meshPoses = new List<Pose>();
		List<string> meshHandles = new List<string>();
		int meshCount = 0;

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

			UIDisplay();
			RenderMeshes();
			MeshUIs();
            
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
					Mesh m = Mesh.GenerateCube(Vec3.One*0.04f);
					meshList.Add(m);
					Pose mPose = new Pose(0, 0, -0.5f, Quat.Identity);
					meshPoses.Add(mPose);
					string mHandle = "Mesh" + meshCount.ToString();
					meshHandles.Add(mHandle);
					meshCount++;
				}
				UI.WindowEnd();
			}
        }//UIDisplay

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
					meshPoses[i] = meshPose;
				}
            }
        }//MeshUIs
    }
}