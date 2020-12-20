# Sprite Collider
![image](https://user-images.githubusercontent.com/4514574/102705555-7b35ac80-4289-11eb-99cf-00bb53b4c5d3.png)  

Very simple tool to create collider data for sprite animations.  
Easier setup of hitboxes for iframes or hurtboxes to deal damage on specific frames.

### Features:
- Create colliders that map one to one with the pixel size of your sprites
- Works with built in animator, although you can use it in other contexts
- Poll an event and it will automatically use the correct frame and collider type
- Hitbox and Hurtbox
- Extensible to support more features like events
- Supports flipping the SpriteCollider
- Preview collider in scene view

### Known Issues:
- Currently the window size is locked to 1024x1024

---

## Usage

#### Open Tool Window
- Window can be opened via the "Tools/Sprite Collider" menu in the top menu bar.
- You can also open `SpriteColliderData` assets by double clicking them in the project window.

#### Create
- Drag an `AnimationClip` onto the Sprite Collider window.

#### Add Collider Type
- Click the Add button in the window and select the collider type  
- Each collider gets a keyframe for each frame in the animation clip  
- Right click the keyframe to enable/disable it  

#### Edit collider/frame
- Select one of the keyframes in the lower area, the selected keyframe will have a small green circle inside it  
- The top area will show a box that shares the color with the collider type  
- Move the box by right clicking and dragging it  
- Expand the box by left click and drag one of the edges

#### Preview the Sprite Collider in Scene View
![image](https://user-images.githubusercontent.com/4514574/102705633-f303d700-4289-11eb-8290-09a742ee52c8.png)
- Add the `SpriteColliderPreview` component to a `GameObject` in the scene and drag the `SpriteRenderer` onto it
- Open a relevant `SpriteColliderData` and have the `GameObject` selected
- The selected frame and collider box is show in the scene view

#### Use the SpriteCollider in game
- Add both the `SpriteCollider` and `Hitbox` component to a GameObject
- Assign the needed components to both of them 
    - `Animator` and `SpriteRenderer` for `SpriteCollider` 
    - A `BoxCollider2D` for `Hitbox`  
- Assign all the `SpriteColliderData` assets to the `SpriteCollider` component, they will automatically map to the AnimationClip it was created with
    - if no `SpriteColliderData` exists for an animation clip no hurtbox exist, but the hitbox will still work. It will use the default size of the `BoxCollider2D` assigned to it.
- Hitbox data is set from the current animation playing and can be listened and used the usual Unity way
- Hurtbox data can be polled by calling `SpriteCollider::CheckEvent(eventType, ...)`

---
( sprite art in images is from [rvros](https://rvros.itch.io/)'s adventurer pack )
