------------------------------------------------------------------
Cubix 1.0.1
------------------------------------------------------------------

	3D Pixel-art package, ease of use for creating various types of casual/mini games.

	Cubix offers two kinds of assets for different purposes, Optimized and Non-optimized assets.

	The Optimized assets let you start develop a project with less Draw Calls at first place.

	The Non-optimized assets can be customized possible way in the development time then you optimize your scene later.

	Features: 

		• 170 Optimized prefabs with shared textures.
		• 170 None-optimized prefabs for customization.
		• Pixel-art textures.
		• A skybox.
		• Demo scenes for Non-optimized and Optimized assets.
		• Mobile and browser friendly.

		• Support all build player platforms.
		
	Compatible:

		• Unity 5.6.1 or higher.

	Youtube:

		https://youtu.be/PKEM3Pjy9Kk

	WebGL Demo:

		https://www.ge-team.com/webgl_demo/cubix/

	Unity Asset Store:

		https://www.assetstore.unity3d.com/#!/content/92184


	Please direct any bugs/comments/suggestions to geteamdev@gmail.com.
		
	Thank you for your support,

	Gold Experience Team
	E-mail: geteamdev@gmail.com
	Website: https://www.ge-team.com

------------------------------------------------------------------
Cubix
------------------------------------------------------------------

	Cubix is a combination of Cube and Pixel-Art. Most 3d models in Cubix based on 1x1x1 Unity cube, the cube can be duplicated or customized for development then they can be baked/optimized for production. Cubix's textures are Pixel-Art designed with minimal colors.

------------------------------------------------------------------
Using Demo scenes
------------------------------------------------------------------

	1. Open Build Settings dialog (Ctrl+Shift+B or Cmd+Shift+B)
	2. Drop demo scenes into Scene In Build section on the Build Settings dialog.
		- For non-optimized scene, use all scenes in "Assets\Cubix\Non-optimized\Scenes" folder.
		- For optimized scene, use all scenes in "Assets\Cubix\Optimized\Scenes" folder.
	3. Open 00_Demo scene or 00_Demo_Optimized depends on kind of demo you want to try.
	4. Press Play button on Unity Editor, you should see the demo works.
	5. Use UI Buttons in Game tab to change sample and rotate the camera and use mouse wheel to zoom.
	6. Hot keys.
		- H key, toggle UI.
		- Left arrow or A key, previous sample.
		- Right arrow or D key, next sample.
		- Up arrow or W key, zoom in.
		- Down arrow or S key, zoom out.

------------------------------------------------------------------
Assets
------------------------------------------------------------------

	Cubix contains two parts, Optimized and Non-optimized assets.

	1. Optimized assets

		The Optimized assets use shared textures and materials for all prefabs for reducing Draw calls to produces better performances on mobile devices and browsers. Using optimized assets gives you more comfortable at the first place.

		However, monitoring the performance and usually things to do like checking and clean up in development time is still important to bring your game to the best performance.

		The optimized assets can be found in "Assets\Cubix\Optimized" folder. Prefab are seperated according to using purpose in "Assets\Cubix\Optimized\Prefabs" folder.

	2. Non-optimized assets

		The Non-optimized assets are for creating game that needs textures or materials customization. Please note that before release or use your game in production, you should bake and optimize it for better performance.
		
		The prefabs are seperated by diffence purposes such as Blocks, Buildings, Grounds and Props.

		2.1 Blocks are moving objects such as chess players and sliding wood. You can tweak and rearrange them to be new shapes or stuffs.

			- Basic, 2 prefabs in "Assets\Cubix\Non-optimized\Blocks\Prefabs\Basic" folder.
			- Chess, 12 prefabs in "Assets\Cubix\Non-optimized\Blocks\Prefabs\Chess" folder.
			- Match-3, 9 prefabs in "Assets\Cubix\Non-optimized\Blocks\Prefabs\Match 3" folder.
			- Sliding, 6 prefabs in "Assets\Cubix\Non-optimized\Blocks\Prefabs\Sliding Block" folder.
			- Stack, 20 prefabs in "Assets\Cubix\Non-optimized\Blocks\Prefabs\Stack Tower" folder.
			- Tetris, 28 prefabs in "Assets\Cubix\Non-optimized\Blocks\Prefabs\Tetris" folder.

		2.2 Buildings are tileable-cube for bulding's floors and walls.

			- Floors, 14 prefabs in "Assets\Cubix\Non-optimized\Buildings\Prefabs\Floors" folder.
			- Walls, 22 prefabs in "Assets\Cubix\Non-optimized\Buildings\Prefabs\Walls" folder.

		2.3 Grounds are tileable-cube for ground.

			- Basic, 1 prefabs in "Assets\Cubix\Non-optimized\Grounds\Prefabs\Basic" folder.
			- Colors, 19 prefabs in "Assets\Cubix\Non-optimized\Grounds\Prefabs\Colors" folder.
			- Grounds, 21 prefabs in "Assets\Cubix\Non-optimized\Grounds\Prefabs\Grounds" folder.

		2.4 Props are for decorating your scene. Most of them are none-cubes prefabs such as trees, bushes, rocks.

			- Found 16 prefabs in "Assets\Cubix\Non-optimized\Props\Prefabs" folder.

------------------------------------------------------------------
SetPass Calls Comparison
------------------------------------------------------------------

	Here are average SetPass Calls in Optimized and Non-optimized scenes.
	Both kinds use same scripts, lights, environment settings and use same Game tab resolution at 960x600px, Different part is only the optimized scene uses optimized prefabs.

	- Non-optimized Scenes (Assets\Cubix\Non-optimized\Scenes folder)

			01_Blocks
				- Sliding Blocks				22
				- Stack Tower					27
				- Tetris						44
				- Chess & Match-3				69
			02_Buildings						70
			03_Grounds
				- Textures						103
				- Basic & Colors				75
			04_Props							30
			05_Skybox							33
			06_Sample_Chess						124
			07_Sample_Match3					1126
			08_Sample_SlidingBlock				139
			09_Sample_StackTower				152
			10_Sample_Tetris					93

	- Optimized Scenes (Assets\Cubix\Optimized\Scenes folder)

			01_Blocks_Optimized
				- Sliding Blocks				3
				- Stack Tower					3
				- Tetris						3
				- Chess & Match-3				3
			02_Buildings_Optimized				26
			03_Grounds_Optimized
				- Textures						7
				- Basic & Colors				10
			04_Props_Optimized					3
			05_Skybox							9
			06_Sample_Chess_Optimized			6
			07_Sample_Match3_Optimized			25
			08_Sample_SlidingBlock_Optimized	12
			09_Sample_StackTower_Optimized		5
			10_Sample_Tetris_Optimized			6
		

------------------------------------------------------------------
Recommendation for optimizing performance
------------------------------------------------------------------

	To reduce the SetPass calls is using as less materials as possible in the scene. There are two ways to do it, manually or automatically.		

	1. The Manual Way

		1. Sort all the materials and gather them by the type of shader they are using.
		2. With the materials that share the same shader gather their textures in a bigger texture (atlas texture).
		3. Create one material that will contain the shader and the atlas texture(s) created on step 2.
		4. Remap all the UVs of your meshes that use the shader to fit the coordinates of the atlas texture.
		5. Assign to your remapped meshes the material we created on step 3.

	2. Automatically Way

		There are great tools to do it automatically. Here are some on Unity Asset Store.

		- Pro Draw call Optimizer
			Free: https://www.assetstore.unity3d.com/en/#!/content/16888
			Full: https://www.assetstore.unity3d.com/en/#!/content/54360

		- Mesh Baker
			Free: https://www.assetstore.unity3d.com/en/#!/content/31895
			Full: https://www.assetstore.unity3d.com/en/#!/content/5017

		- Super Level Optmizer
			Free: https://www.assetstore.unity3d.com/en/#!/content/23245
			Full: https://www.assetstore.unity3d.com/en/#!/content/25370

	Note:
		- To monitor SetPass calls on Unity Editor, click on the Stats button in top-right area in Game tab then it appears in the Statistics dialog.
		- More details, https://docs.unity3d.com/Manual/DrawCallBatching.html and https://unity3d.com/learn/tutorials/temas/performance-optimization/optimizing-graphics-rendering-unity-games.	


------------------------------------------------------------------
Release notes
------------------------------------------------------------------

	Version 1.0.1

		• Add a pixel-art skybox.
		• Add new demo scenes for skybox and update some other scenes.
		• Update sample scripts.

	Version 1.0 (Initial version)

		• Contains 170 Optimized prefabs with shared textures.
		• Contains 170 None-optimized prefabs for customization.
		• Pixel-art textures.
		• Demo scenes for Non-optimized and Optimized.
		• Mobile and browser friendly.
		• Support all build player platforms.
		• Unity 5.6.1 or higher.