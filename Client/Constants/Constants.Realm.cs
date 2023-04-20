using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WotlkClient.Constants
{
    public enum LoginErrorCode : byte
    {
        RESPONSE_SUCCESS = 0,
        RESPONSE_FAILURE = 1,
        RESPONSE_CANCELLED = 2,
        RESPONSE_DISCONNECTED = 3,
        RESPONSE_FAILED_TO_CONNECT = 4,
        RESPONSE_CONNECTED = 5,
        RESPONSE_VERSION_MISMATCH = 6,
        CSTATUS_CONNECTING = 7,
        CSTATUS_NEGOTIATING_SECURITY = 8,
        CSTATUS_NEGOTIATION_COMPLETE = 9,
        CSTATUS_NEGOTIATION_FAILED = 10,
        CSTATUS_AUTHENTICATING = 11,
        AUTH_OK = 12,
        AUTH_FAILED = 13,
        AUTH_REJECT = 14,
        AUTH_BAD_SERVER_PROOF = 15,
        AUTH_UNAVAILABLE = 16,
        AUTH_SYSTEM_ERROR = 17,
        AUTH_BILLING_ERROR = 18,
        AUTH_BILLING_EXPIRED = 19,
        AUTH_VERSION_MISMATCH = 20,
        AUTH_UNKNOWN_ACCOUNT = 21,
        AUTH_INCORRECT_PASSWORD = 22,
        AUTH_SESSION_EXPIRED = 23,
        AUTH_SERVER_SHUTTING_DOWN = 24,
        AUTH_ALREADY_LOGGING_IN = 25,
        AUTH_LOGIN_SERVER_NOT_FOUND = 26,
        AUTH_WAIT_QUEUE = 27,//
        AUTH_BANNED = 28,
        AUTH_ALREADY_ONLINE = 29,
        AUTH_NO_TIME = 30,
        AUTH_DB_BUSY = 31,
        AUTH_SUSPENDED = 32,
        AUTH_PARENTAL_CONTROL = 33,
        AUTH_LOCKED_ENFORCED = 34,
        REALM_LIST_IN_PROGRESS = 35,
        REALM_LIST_SUCCESS = 36,
        REALM_LIST_FAILED = 37,
        REALM_LIST_INVALID = 38,
        REALM_LIST_REALM_NOT_FOUND = 39,
        ACCOUNT_CREATE_IN_PROGRESS = 40,//
        ACCOUNT_CREATE_SUCCESS = 41,
        ACCOUNT_CREATE_FAILED = 42,
        CHAR_LIST_RETRIEVING = 43,
        CHAR_LIST_RETRIEVED = 44,
        CHAR_LIST_FAILED = 45,
        CHAR_CREATE_IN_PROGRESS = 46,
        CHAR_CREATE_SUCCESS = 47,
        CHAR_CREATE_ERROR = 48,
        CHAR_CREATE_FAILED = 49,
        CHAR_CREATE_NAME_IN_USE = 50,//
        CHAR_CREATE_DISABLED = 51,
        CHAR_CREATE_PVP_TEAMS_VIOLATION = 52,
        CHAR_CREATE_SERVER_LIMIT = 53,
        CHAR_CREATE_ACCOUNT_LIMIT = 54,
        CHAR_CREATE_SERVER_QUEUE = 55,
        CHAR_CREATE_ONLY_EXISTING = 56,
        CHAR_CREATE_EXPANSION = 57,
        CHAR_CREATE_EXPANSION_CLASS = 58,
        CHAR_CREATE_LEVEL_REQUIREMENT = 59,
        CHAR_CREATE_UNIQUE_CLASS_LIMIT = 60,//
        CHAR_DELETE_IN_PROGRESS = 61,
        CHAR_DELETE_SUCCESS = 62,
        CHAR_DELETE_FAILED = 63,
        CHAR_DELETE_FAILED_LOCKED_FOR_TRANSFER = 64,
        CHAR_DELETE_FAILED_GUILD_LEADER = 65,
        CHAR_DELETE_FAILED_ARENA_CAPTAIN = 66,
        CHAR_LOGIN_IN_PROGRESS = 67,
        CHAR_LOGIN_SUCCESS = 68,
        CHAR_LOGIN_NO_WORLD = 69,
        CHAR_LOGIN_DUPLICATE_CHARACTER = 70,//
        CHAR_LOGIN_NO_INSTANCES = 71,
        CHAR_LOGIN_FAILED = 72,
        CHAR_LOGIN_DISABLED = 73,
        CHAR_LOGIN_NO_CHARACTER = 74,
        CHAR_LOGIN_LOCKED_FOR_TRANSFER = 75,
        CHAR_LOGIN_LOCKED_BY_BILLING = 76,
        CHAR_NAME_SUCCESS = 77,
        CHAR_NAME_FAILURE = 78,
        CHAR_NAME_NO_NAME = 79,
        CHAR_NAME_TOO_SHORT = 80,//
        CHAR_NAME_TOO_LONG = 81,
        CHAR_NAME_INVALID_CHARACTER = 82,
        CHAR_NAME_MIXED_LANGUAGES = 83,
        CHAR_NAME_PROFANE = 84,
        CHAR_NAME_RESERVED = 85,
        CHAR_NAME_INVALID_APOSTROPHE = 86,
        CHAR_NAME_MULTIPLE_APOSTROPHES = 87,
        CHAR_NAME_THREE_CONSECUTIVE = 88,
        CHAR_NAME_INVALID_SPACE = 89,
        CHAR_NAME_CONSECUTIVE_SPACES = 90,//
        CHAR_NAME_RUSSIAN_CONSECUTIVE_SILENT_CHARACTERS = 91,
        CHAR_NAME_RUSSIAN_SILENT_CHARACTER_AT_BEGINNING_OR_END = 92,
        CHAR_NAME_DECLENSION_DOESNT_MATCH_BASE_NAME = 93,
    }

    public struct Character
    {
        public UInt64 GUID;
        public string Name;
        public byte Race;
        public byte Class;
        public byte Level;
        public UInt32 MapID;
        
    }

    public struct Realm
    {
        public byte Type;
        public byte Color;
        public byte NameLen;
        public string Name;
        public byte AddrLen;
        public string Address;
        public float Population;
        public byte NumChars;
        public byte Language;
        public byte Unk; // const: 1
    }
}
