using System;
using System.Collections.Generic;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace PlogonScript.Script;

public static class Events
{
    public static readonly Event OnLoad = new("onLoad");
    public static readonly Event OnUnload = new("onUnload");
    public static readonly Event OnDraw = new("onDraw");
    public static readonly Event OnUpdate = new("onUpdate");

    public static readonly Event OnKeyUp = new("onKeyUp",
        new Dictionary<string, Type> {{"key", typeof(VirtualKey)}});

    public static readonly Event OnChatMessageUnhandled = new("onChatMessageUnhandled",
        new Dictionary<string, Type>
        {
            {"type", typeof(XivChatType)}, {"senderId", typeof(uint)}, {"sender", typeof(SeString)},
            {"message", typeof(SeString)}
        });

    public static readonly Event[] AllEvents = {OnLoad, OnUnload, OnDraw, OnUpdate, OnChatMessageUnhandled};
}