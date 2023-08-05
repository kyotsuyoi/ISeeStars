namespace ISS
{
    public enum EnumGameObjectType
    {
        Default,
        Health,
        Oxygen,
        Energy,
        WoodenBox,
        MetalWall
    }

    public enum EnumGameMenuType
    {
        Main,
        Settings,
        MachineDefault
    }

    public enum EnumInteractionType
    {
        None,
        MachineDefault
    }

    public enum EnumSoundFX : int
    {
        None = 0,
        PlayerTakeDamage1 = 1,
        PlayerTakeDamage2 = 2,
        PlayerDead = 3,
        Jump = 4,
        MenuNavigation = 5,
        MenuSelected = 6,
        MenuOpen = 7,
        MenuNotOpen = 8,
        MenuClose = 9,
        TouchingGround = 10,
        JetPack = 11,
        Hit = 12,
        PlayerSuffocating = 13
    }

    public enum EnumSoundOrigin
    {
        None,
        Player
    }
}
 