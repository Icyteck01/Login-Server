public static class CommonConstant
{
    public const int SUCCESS = 0;
    public const int FAILURE = 1;
    public const int TARGET_NOT_FOUND = 6;
    public const int TARGET_DEADED = 7;
    public const int CASTER_NOT_FOUND = 8;
    public const int CASTER_DEADED = 9;
    public const int SKILL_NOT_FOUND = 10;
    public const int POSITION_INVALID = 11;
    public const int COOL_TIMING = 12;
    public const int SKILL_NO_MANA = 13;
    public const int UNKNOWN_SKILL = 14;
    public const int ITEM_TYPE_MISSMACH = 15;
    public const int ITEM_NOT_FOUND = 16;
    public const int ITEM_VFX_INUSE = 17;
    public const int ITEM_NOT_ENOUGH = 18;
    public const int ITEM_CONFIG_WRONG = 19;
    public const int ITEM_COOLING = 20;
    public const int INVENTORY_NOT_FOUND = 21;
    public const int INVENTORY_FULL = 22;
    public const int INVENTORY_FULL_RESULT = 23;
    public const int ITEM_NOT_IN_INVENTORY = 24;
    public const int INVENTORY_ITEM_ALREADY_ON = 25;
    public const int INVENTORY_ITEM_MOVED = 27;
    public const int INVENTORY_ITEM_NOT_MOVED = 28;
    public const int INVENTORY_SUCCESS_MULTIPLE = 29;
    public const int INVENTORY_ITEM_NOT_FOUND = 30;
    public const int ITEM_EXPIRED = 31;
    public const int ITEM_EQUIPED = 32;
    public const int TARGET_TO_FAR = 33;
    public const int HERO_PROPS_CANT_USE_STATS_FULL = 34;
    public const int BACKPACK_ALREADYON = 35;
    //LOGIN
    public const int INPUT_LOGIN_WRONG = 36;
    public const int INPUT_PASSWORD_WRONG = 37;
    public const int USER_NOT_FOUND = 38;
    public const int DISCONNECTED_FROM_LOGIN_SERVER = 39;
    public const int DISCONNECTED_FROM_LOGIN_SERVER_PERMANENT = 40;
    public const int RECCONNECTING_TO_LOGIN = 41;
    public const int WRONG_PASSWORD_OR_LOGIN = 42;

    public const int NOT_ENOUGH_GOLD = 43;
    public const int NOT_ENOUGH_SILVER = 44;
    public const int PLAYER_NOT_FOUND = 45;
    public const int NOT_DELETE_LAST_CHARACTER = 46;
    public const int ACTIVE_ALREADY_COLLECTED = 47;
    public const int ACTIVE_NOT_FOUND = 48;
    public const int ACTIVE_NOT_COMPLEATED = 49;

    public enum INV_PACKET
    {
        NONE, REMOVE, REMOVE_AND_MOVE, REMOVE_AND_EQUIP, EQUIP, MOVE, MOVE_AND_EQUIP, REMOVE_AND_UNEQUIP, UNEQUIP, UNEQUIP_AND_EQUIP, USE_ITEM,
        THROW_ITEM
    }
}

