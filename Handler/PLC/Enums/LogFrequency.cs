using System.ComponentModel.DataAnnotations;

namespace OptimaValue
{
    public enum LogFrequency
    {
        Never = 0,
        _50ms = 50,
        _100ms = 100,
        _250ms = 250,
        _500ms = 500,
        _1s = 1000,
        _2s = 2000,
        _5s = 5000,
        _10s = 10000,
        _30s = 30000,
        _1m = 60000,
    }
}
