# Hat

This is an area where you can often wrestle with the Unity Editor.

The documentation is quite long,
to allow you to create hat customizations to a minimum standard.

*This guide is based on Blender (4.3).*

## If you want to skip entire basic stuff...

This guide follows a rough Blender basics.  
So if you're already familiar with it,
you only need to know the core information below
to create your own resources.

- This mod only allows 2 textures total.
  - Recommended to use `512x512`, `1024x1024` pixels.
- Requires passport menu icon's texture separately.
  - Recommended to use `128x128` pixels.
- `Y+` should be forward.
- Scout's head radius is approximately `1.415` units.
- World's origin point is center of scout's head.
- Recommended to export `.fbx`.

![Export](./img/hat-guide-12.png)
- When you export, open 'Transform' tab and apply below settings.
  - Forward : Y Forward
  - Up : Z Up
  - Uncheck 'Apply Unit'
  - Check 'Use Space Transform' and 'Apply Transform'
- Save textures to `.png` file if you made mapped to object.

[Now you are ready to build asset bundle!](https://github.com/Creta5164/peak-more-customizations/tree/main/docs/build-asset-bundle.md)

## Prepare scene in Blender

![Two collections](./img/hat-guide-1.png)

1. Create two collections, one is for guideline, and other one is for actual hat model.
   - I'd recommend to name them like below.
     - `Guideline Collection`
     - `Your Collection`

![Creating sphere](./img/hat-guide-2.png)

2. Select `Guideline Collection` and then create sphere object.  
   (Creating shortcut is <kbd>Shift</kbd> + <kbd>A</kbd> by default)

![Setting radius](./img/hat-guide-3.png)

3. At bottom left corner, you can see `Add UV Sphere` option.  
   Click to open it and set `Radius` to `1.415`.  
   This is approximately radius of scout's head.

![You can go now](./img/hat-guide-4.png)

4. Now, you can do whatever you want to make own hat in `Your Collection`!

![Which way is forward](./img/hat-guide-5.png)

You can predicate forward direction by upper right side of this gizmo.

## Painting (texture)

Note that you can only use 2 textures total!

![Creating textured material](./img/hat-guide-6.gif)

1. Create material you want to paint in desired object.  
   Recommended texture size is `512x512` pixels.

![Unwrap UV](./img/hat-guide-7.gif)

2. Go to `UV Editing` at top of layout menu.  
   Order to paint texture, you should unwrap UV of your object.

![Paint texture in Blender](./img/hat-guide-8.gif)

3. Now, go to `Texture Paint` at top of layout menu.  
   You can draw on object!

![Use external texture](./img/hat-guide-9.png)

If you want to draw with your favorite image/drawing software,
You can import texture from 'Image/Open...' menu.

You can take a screenshot of the area where the mesh is drawn
in the Paint View on the left and use it as a guide
for texture mapping.

## Export

Let's prepare hat model for Unity.

![Disable guideline objects](./img/hat-guide-10.png)

1. Before we export, go to scene hierarchy.  
   Press checkbox of `Guideline Collection` to exclude them.

![Export menu](./img/hat-guide-11.png)

2. On the top, go to 'File/Export/FBX (.fbx)' menu.

![Export options](./img/hat-guide-12.png)

3. When you export, open 'Transform' tab and apply below settings.
  - Forward : Y Forward
  - Up : Z Up
  - Uncheck 'Apply Unit'
  - Check 'Use Space Transform' and 'Apply Transform'

![Save textures](./img/hat-guide-13.png)

4. Lastly, export textures you've made.
   
![More textures](./img/hat-guide-14.png)

If you made more then one, You can find photo dropdown icon
at left side of texture name.

![Icon](./img/hat-guide-15.png)

5. Make icon for it!  
   This is required for displaying in passport menu.
   - Recommended size is `128x128` pixels.
   

[Now you are ready to build asset bundle!](build-asset-bundle.md)
