using System;
using System.ComponentModel;
using Newtonsoft.Json;

public class NetworkAttribute<T>
{
	public event PropertyChangedEventHandler networkValueChange = delegate {};
	public event PropertyChangedEventHandler valueChange = delegate {};
	
	private T value;
	public T Value
	{
		get
		{
			return value;
		}
		set
		{
			networkValueChange(Id, new PropertyChangedEventArgs(JsonConvert.SerializeObject(value)));
		}
	}
	public string Id {get;}
	public NetworkAttribute(string id, T value)
	{
		this.Id = id;
		this.value = value;
	}
	public void NonNetworkedSet(T value)
	{
		this.value = value;
		valueChange(this, new PropertyChangedEventArgs("NonNetworkChange"));
	}
	
}