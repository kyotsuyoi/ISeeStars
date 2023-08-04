namespace ISS
{    
    public enum EnumGameObjectType
    {
        Default,
        Health,
        Oxygen,
        Energy
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

    public enum EnumSoundFX: int
    {
        None = 0,
        PlayerTakeDamage1 = 1,
        PlayerTakeDamage2 = 2,
        PlayerDead = 3,
        PlayerJump1 = 4,
        PlayerJump2 = 5,
        MenuNavigation = 6,
        MenuSelected = 7,
        MenuOpen = 8,
        MenuNotOpen = 9,
        TouchingGround = 10
    }
}
 