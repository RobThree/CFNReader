using System.Collections.Generic;

namespace CFNReader;

public class ValueCollection(IDictionary<Channel, UnitValue> values) : Dictionary<Channel, UnitValue>(values)
{ }