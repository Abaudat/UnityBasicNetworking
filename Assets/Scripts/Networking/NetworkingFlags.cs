using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetworkingFlags {

	public enum Flags : byte
    {
        DEBUG_MESSAGE, //sends a message that will be shown in the debug log.
        GAME_ID
    }
}
