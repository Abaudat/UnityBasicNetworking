using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing flags for each type of networking message.
/// </summary>
public static class NetworkingFlags {

    /// <summary>
    /// Add flags to this enum for each type of networking message.
    /// </summary>
	public enum Flags : byte
    {
        DEBUG_MESSAGE // Send a message that will be shown in the debug log.
    }
}
