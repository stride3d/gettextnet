namespace GNU.Gettext.Xgettext;
public enum ExtractMode
{
    Msgid,
    MsgidConcat, // Like "Str 1 " + "Str 2" + "Str 3"
    MsgidFromResx, // For forms/controls that have property "Localizable" = true
    MsgidPlural,
    ContextMsgid,
}

