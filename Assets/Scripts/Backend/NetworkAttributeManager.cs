

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
public class GenericDictionary
{
    private Dictionary<string, object> _dict = new Dictionary<string, object>();

    public void Add<T>(string key, T value)
    {
        _dict.Add(key, value);
    }

    public T GetValue<T>(string key) where T : class
    {
        return _dict[key] as T;
    }
}

public static class NetworkAttributeManager
{
	public static event PropertyChangedEventHandler valueChanged = delegate {};
	private static GenericDictionary networkAttributes = new GenericDictionary();
	private static int _id = 0;

    static void AttributeChangedEventHandler(object sender, PropertyChangedEventArgs e)
    {
        valueChanged(sender, e);
    }
	public static NetworkAttribute<T> AddNetworkAttribute<T>(string uuid, T value)
	{
        string newID = $"{uuid}-{_id++}";
		NetworkAttribute<T> newAttribute = new NetworkAttribute<T>(newID, value);
        networkAttributes.Add<T>(newID,value);
        newAttribute.networkValueChange += AttributeChangedEventHandler;
		return newAttribute;
	}

}