# Adding custom charms

The `CharmHelper` class is used to add custom charms to the inventory.

A good example of a mod adding custom charms is the [MoreHealing](https://github.com/SFGrenade/MoreHealing) mod.

## Loading charm sprites

First thing's first is to load the sprites you want your charms to have.

You can do so doing:
```cs
private static void Sprite LoadSprite() {
    Assembly asm = Assembly.GetExecutingAssembly();
    Sprite charmSprite = null;
    using (Stream s = asm.GetManifestResourceStream("AssemblyName.Subfolder.Filename.png"))
    {
        if (s == null) continue;
        byte[] buffer = new byte[s.Length];
        s.Read(buffer, 0, buffer.Length);
        s.Dispose();
        // Create texture from bytes
        var tex = new Texture2D(2, 2);
        tex.LoadImage(buffer, true);
        // Create sprite from texture
        charmSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
    // Now `charmSprite` contains the sprite, or null if it couldn't be found
    return charmSprite;
}
```

## Registering new charms

After we have all the important sprites loaded, we can supply them to `CharmHelper`.

For this, we use the `CharmHelper.AddSprites(...)` method:
```cs
public List<int> CharmIDs { get; private set; }
private Sprite CharmSprite = LoadSprite();
public override void Initialize()
{
    CharmIDs = CharmHelper.AddSprites(CharmSprite);
    //InitCallbacks();
}
```

This method takes any amount of `Sprite`s.

> At the moment, charms that have multiple sprites for different states are not supported.

The `List<int>` the method returns is a list of all charm IDs of the charms you have registered.

The handling of name, description, aquisition and cost of each charm is for the individual mod to handle, but here is one example.

## Handling charm metadata

Charm metadata (name, description, aquisition and cost) has to be handled by mods, all of which can be done with `ModHooks`:
```cs
private void InitCallbacks()
{
    // name and description
    ModHooks.LanguageGetHook += OnLanguageGetHook;
    // aquisition
    ModHooks.GetPlayerBoolHook += OnGetPlayerBoolHook;
    // aquisition
    ModHooks.SetPlayerBoolHook += OnSetPlayerBoolHook;
    // cost
    ModHooks.GetPlayerIntHook += OnGetPlayerIntHook;
}
```

!!! todo !!!
