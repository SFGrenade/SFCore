# Adding custom charms

## CharmHelper

The `CharmHelper` class is used to add custom charms to the inventory.

A good example of a mod adding custom charms is the [MoreHealing](https://github.com/SFGrenade/MoreHealing) mod.

### Loading charm sprites

First thing's first is to load the sprites you want your charms to have.

You can do so doing:
```cs
private static void Sprite LoadSprite(string spriteName) {
    // Important to get embedded resources
    Assembly asm = Assembly.GetExecutingAssembly();
    // Will contain the sprite
    Sprite charmSprite = null;
    // Open an embedded resource as a stream
    using (Stream s = asm.GetManifestResourceStream($"AssemblyName.Subfolder.{spriteName}.png"))
    {
        // In case of typos do nothing
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

### Registering new charms

After we have all the important sprites loaded, we can supply them to `CharmHelper`.

For this, we use the `CharmHelper.AddSprites(...)` method:
```cs
// This will contain the charm id of our charms
public List<int> CharmIDs { get; private set; }
// These will have the sprites we are going to use
private Sprite Charm1Sprite = LoadSprite("Charm1");
private Sprite Charm2Sprite = LoadSprite("Charm2");
public override void Initialize()
{
    CharmIDs = CharmHelper.AddSprites(Charm1Sprite, Charm2Sprite);
    // Will be explained in a bit
    InitCallbacks();
}
```

This method takes any amount of `Sprite`s.

> At the moment, charms that have multiple sprites for different states are not supported.

The `List<int>` the method returns is a list of all charm IDs of the charms you have registered.

The handling of name, description, aquisition and cost of each charm is for the individual mod to handle, but here is one example.

### Handling charm metadata

Charm metadata (name, description, aquisition and cost) has to be handled by mods, all of which can be done with `ModHooks`:
```cs
private void InitCallbacks()
{
    // This hook will handle name and description
    ModHooks.LanguageGetHook += OnLanguageGetHook;
    // This hook will handle getting aquisition
    ModHooks.GetPlayerBoolHook += OnGetPlayerBoolHook;
    // This hook will handle setting aquisition
    ModHooks.SetPlayerBoolHook += OnSetPlayerBoolHook;
    // This hook will handle cost
    ModHooks.GetPlayerIntHook += OnGetPlayerIntHook;
}
```

These look like a lot, but can be done relatively easily.

#### Name and description

The following is one example how to handle the name and description of the custom charms.

```cs
// Contains the names we want our charms to have
private string[] _charmNames =
{
    "Charm1 Name",
    "Charm2 Name"
};
// Contains the descriptions we want our charms to have
private string[] _charmDescriptions =
{
    "Charm1 Description",
    "Charm2 Description"
};
private string OnLanguageGetHook(string key, string sheet, string orig)
{
    // Check if the charm name is wanted
    if (key.StartsWith("CHARM_NAME_"))
    {
        // Split CHARM_NAME_# into ('CHARM', 'NAME', '#') and parse '#' as an int
        int charmNum = int.Parse(key.Split('_')[2]);
        // Check if `charmNum` is one of our registered charm IDs
        if (CharmIDs.Contains(charmNum))
        {
            // Return the name of our charm from the array above
            return _charmNames[CharmIDs.IndexOf(charmNum)];
        }
    }
    // Check if the charm description is wanted
    else if (key.StartsWith("CHARM_DESC_"))
    {
        // Split CHARM_DESC_# into ('CHARM', 'DESC', '#') and parse '#' as an int
        int charmNum = int.Parse(key.Split('_')[2]);
        // Check if `charmNum` is one of our registered charm IDs
        if (CharmIDs.Contains(charmNum))
        {
            // Return the description of our charm from the array above
            return _charmDescriptions[CharmIDs.IndexOf(charmNum)];
        }
    }
    // Return orig if we don't want to make any changes
    return orig;
}
```

> This is one example, alternatives could be to load the text from a JSON file or comparable and use the loaded text.  
> One example is how [Test of Teamwork handles language](https://github.com/SFGrenade/TestOfTeamwork/blob/a07d033b5dcde76314d0144ad0f9aa8399e10500/TestOfTeamwork.cs#L248-L252), which it does [with a JSON deserializer](https://github.com/SFGrenade/TestOfTeamwork/blob/a07d033b5dcde76314d0144ad0f9aa8399e10500/Consts/LanguageStrings.cs)

#### Getting aquisition

The following is one example how to handle getting the aquisition of the custom charms.

One thing to note, is that we need a class for save settings if we want our charms to save across play sessions.

This save settings class can look like the following.
```cs
class ModSaveSettings
{
    // Flags if the charms have been aquired by default
    public bool[] gotCharms = new[] { false, false };
    // Flags if the charms are considered new (they will show a small blue dot in the inventory)
    public bool[] newCharms = new[] { false, false };
    // Flags if the charms are equipped
    public bool[] equippedCharms = new[] { false, false };
    // How many notches the charms cost
    public int[] charmCosts = new[] { 1, 2 };
}
```

This save settings class can be used by the mod by having the mod declaration utilize the `SaveSettingsMod<T>` class.
```cs
class ModClass : SaveSettingsMod<ModSaveSettings>
```
Instead of the normal:
```cs
class ModClass : Mod
```

After this has been done, we can implement the modhooks.
```cs
private bool OnGetPlayerBoolHook(string target, bool orig)
{
    // Check if the charm gotten flag is wanted
    if (target.StartsWith("gotCharm_"))
    {
        // Split gotCharm_# into ('gotCharm', '#') and parse '#' as an int
        int charmNum = int.Parse(target.Split('_')[1]);
        // Check if `charmNum` is one of our registered charm IDs
        if (CharmIDs.Contains(charmNum))
        {
            // Return the flag if our charm has been aquired
            return SaveSettings.gotCharms[CharmIDs.IndexOf(charmNum)];
        }
    }
    // Check if the charm new flag is wanted
    if (target.StartsWith("newCharm_"))
    {
        // Split newCharm_# into ('newCharm', '#') and parse '#' as an int
        int charmNum = int.Parse(target.Split('_')[1]);
        // Check if `charmNum` is one of our registered charm IDs
        if (CharmIDs.Contains(charmNum))
        {
            // Return the flag if our charm is considered new
            return SaveSettings.newCharms[CharmIDs.IndexOf(charmNum)];
        }
    }
    // Check if the charm equipped flag is wanted
    if (target.StartsWith("equippedCharm_"))
    {
        // Split equippedCharm_# into ('equippedCharm', '#') and parse '#' as an int
        int charmNum = int.Parse(target.Split('_')[1]);
        // Check if `charmNum` is one of our registered charm IDs
        if (CharmIDs.Contains(charmNum))
        {
            // Return the flag if our charm is equipped
            return SaveSettings.equippedCharms[CharmIDs.IndexOf(charmNum)];
        }
    }
    // Return orig if we don't want to make any changes
    return orig;
}
```

#### Setting aquisition

The following is one example how to handle setting the aquisition states of the custom charms.

```cs
private bool OnSetPlayerBoolHook(string target, bool orig)
{
    // Check if the charm gotten flag is wanted
    if (target.StartsWith("gotCharm_"))
    {
        // Split gotCharm_# into ('gotCharm', '#') and parse '#' as an int
        int charmNum = int.Parse(target.Split('_')[1]);
        // Check if `charmNum` is one of our registered charm IDs
        if (CharmIDs.Contains(charmNum))
        {
            // Save the flag if our charm has been aquired
            SaveSettings.gotCharms[CharmIDs.IndexOf(charmNum)] = orig;
            // Always return orig in set hooks, unless you specifically want to change what is saved
            return orig;
        }
    }
    // Check if the charm new flag is wanted
    if (target.StartsWith("newCharm_"))
    {
        // Split newCharm_# into ('newCharm', '#') and parse '#' as an int
        int charmNum = int.Parse(target.Split('_')[1]);
        // Check if `charmNum` is one of our registered charm IDs
        if (CharmIDs.Contains(charmNum))
        {
            // Save the flag if our charm is considered new
            SaveSettings.newCharms[CharmIDs.IndexOf(charmNum)] = orig;
            // Always return orig in set hooks, unless you specifically want to change what is saved
            return orig;
        }
    }
    // Check if the charm equipped flag is wanted
    if (target.StartsWith("equippedCharm_"))
    {
        // Split equippedCharm_# into ('equippedCharm', '#') and parse '#' as an int
        int charmNum = int.Parse(target.Split('_')[1]);
        // Check if `charmNum` is one of our registered charm IDs
        if (CharmIDs.Contains(charmNum))
        {
            // Save the flag if our charm is equipped
            SaveSettings.equippedCharms[CharmIDs.IndexOf(charmNum)] = orig;
            // Always return orig in set hooks, unless you specifically want to change what is saved
            return orig;
        }
    }
    // Always return orig in set hooks, unless you specifically want to change what is saved
    return orig;
}
```

#### Cost

The following is one example how to handle the cost of our custom charms.

```cs
private int OnGetPlayerIntHook(string target, int orig)
{
    // Check if the charm cost is wanted
    if (target.StartsWith("charmCost_"))
    {
        // Split charmCost_# into ('charmCost', '#') and parse '#' as an int
        int charmNum = int.Parse(target.Split('_')[1]);
        // Check if `charmNum` is one of our registered charm IDs
        if (CharmIDs.Contains(charmNum))
        {
            // Return the cost of our charm
            return SaveSettings.charmCosts[CharmIDs.IndexOf(charmNum)];
        }
    }
    // Return orig if we don't want to make any changes
    return orig;
}
```

### Done

With handling of the charm metadata done, the basics of custom charms are done.
They can be equipped and unequipped.

If you want your charms to have special effects, that is more complicated and it is suggested to look at how other mods do it.

## EasyCharm

The abstract `EasyCharm` class can be used to more simply add custom charms.

A good example of a mod adding custom charms is the [EasyCharmPrototype](https://github.com/PrashantMohta/EasyCharmPrototype) mod.

### Registering new charms

After we have all the important sprites loaded, we can construct the class subclassing `EasyCharm`:

```cs
public class CustomCharm1 : EasyCharm
{
    protected override int GetCharmCost() => 1;

    protected override string GetDescription() => "Charm1 Description";

    protected override string GetName() => "Charm1 Name";

    protected override Sprite GetSpriteInternal()=> AssemblyUtils.GetSpriteFromResources("Charm1.png");
}
```

Once the class is declared, we can use a instance of this class in the mod class:

```cs
/* inside class `CharmMod1` */
internal Dictionary<string, EasyCharm> Charms = new Dictionary<string, EasyCharm>{
    {"CustomCharm1", new CustomCharm1()},
};
```

#### Setting aquisition

The following is one example how to handle setting the aquisition states of the custom charms.

```cs
public class CharmMod1Settings {
    public Dictionary<string, EasyCharmState> Charms;
}
public class CharmMod1 : Mod, ILocalSettings<CharmMod1Settings> {
    public CharmMod1Settings SaveSettings = new CharmMod1Settings();
    public void OnLoadLocal(settings s)
    {
        SaveSettings = s;
        if (s.Charms != null)
        {
            foreach(var kvp in s.Charms)
            {
                if (Charms.TryGetValue(kvp.Key, out EasyCharm m))
                {
                    m.RestoreCharmState(kvp.Value);
                }
            }
        }
    }
    public settings OnSaveLocal()
    {
        SaveSettings.Charms = new Dictionary<string, EasyCharmState>();
        foreach (var kvp in Charms)
        {
            if (Charms.TryGetValue(kvp.Key, out EasyCharm m))
            {
                SaveSettings.Charms[kvp.Key] = m.GetCharmState();
            }
        }
        return SaveSettings;
    }
}
```
