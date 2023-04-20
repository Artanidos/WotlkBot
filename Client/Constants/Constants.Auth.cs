using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WotlkClient.Constants
{
    /// <summary>
    /// Realm Auth Proof Error Codes
    /// </summary>
    public enum AccountStatus : byte
    {
        Ok = 0x00,
        IPBanned = 0x01,
        Banned = 0x03,
        UnknownAccount = 0x04,
        AlreadyOnline = 0x06,
        NoTimeLeft = 0x07,
        DBBusy = 0x08,
        BadVersion = 0x09,
        PatchRequired = 0x0A,
        AccountFrozen = 0x0C,
        ParentalControl = 0x0F
    }
}
