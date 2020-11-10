using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatStack
{
    private Dictionary<HatBehavior.HatType, int> _hatTypeNumber = new Dictionary<HatBehavior.HatType, int>();
    private Stack<Hat> _activeHats = new Stack<Hat>();

    public HatStack()
    {

    }

    public Hat Top => _activeHats.Peek();
    public Hat Pop()
    {
        Hat hatToPop = _activeHats.Pop();
        if (_hatTypeNumber.ContainsKey(hatToPop.Type))
        {
            _hatTypeNumber[hatToPop.Type] -= 1;
        }
        return hatToPop;
    }

    public void AddHat(Hat hatType)
    {
        _activeHats.Push(hatType);
        if (!_hatTypeNumber.ContainsKey(hatType.Type))
        {
            _hatTypeNumber.Add(hatType.Type, 1);
        }
        else _hatTypeNumber[hatType.Type] += 1;
    }

    public int GetNumHat(HatBehavior.HatType type) {
        if (!_hatTypeNumber.ContainsKey(type)) return 0;
        return _hatTypeNumber[type];
    }

    public bool HasHats => _activeHats.Count > 0;

    public int NumHats => _activeHats.Count;
}
