#region Assembly System.Windows.dll, v2.0.50727
// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\System.Windows.dll
#endregion

#if !UNITY_METRO || UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace System.Collections.ObjectModel.Reign
{
	// Summary:
	//     Represents a dynamic data collection that provides notifications when items
	//     get added, removed, or when the entire list is refreshed.
	//
	// Type parameters:
	//   T:
	//     The type of items in the collection.
	public class ObservableCollection<T> : List<T>//, INotifyCollectionChanged, INotifyPropertyChanged
	{
		/*// Summary:
		//     Initializes a new, empty instance of the System.Collections.ObjectModel.ObservableCollection<T>
		//     class.
		public ObservableCollection();
		//
		// Summary:
		//     Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection<T>
		//     class and populates it with items copied from the specified collection.
		//
		// Parameters:
		//   collection:
		//     The collection from which the items are copied.
		public ObservableCollection(IEnumerable<T> collection);
		//
		// Summary:
		//     Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection<T>
		//     class and populates it with items copied from the specified list.
		//
		// Parameters:
		//   list:
		//     The list from which the items are copied.
		public ObservableCollection(List<T> list);

		// Summary:
		//     Occurs when an item in the collection changes, or the entire collection changes.
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		//
		// Summary:
		//     Occurs when a property on an individual item in the collection changes.
		protected event PropertyChangedEventHandler PropertyChanged;

		// Summary:
		//     Removes all items from the collection.
		protected override void ClearItems();
		//
		// Summary:
		//     Inserts an item into the collection at the specified index.
		//
		// Parameters:
		//   index:
		//     The zero-based index at which item should be inserted.
		//
		//   item:
		//     The object to insert.
		protected override void InsertItem(int index, T item);
		//
		// Summary:
		//     Raises the System.Collections.ObjectModel.ObservableCollection<T>.CollectionChanged
		//     event with the provided event data.
		//
		// Parameters:
		//   e:
		//     The event data to report in the event.
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e);
		//
		// Summary:
		//     Raises the System.Collections.ObjectModel.ObservableCollection<T>.PropertyChanged
		//     event with the provided arguments.
		//
		// Parameters:
		//   e:
		//     The event data to report in the event.
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e);
		//
		// Summary:
		//     Removes the item at the specified index from the collection.
		//
		// Parameters:
		//   index:
		//     The zero-based index of the item to remove.
		protected override void RemoveItem(int index);
		//
		// Summary:
		//     Replaces the item at the specified index.
		//
		// Parameters:
		//   index:
		//     The zero-based index of the item to replace.
		//
		//   item:
		//     The new value for the item at the specified index.
		protected override void SetItem(int index, T item);*/
	}
}
#endif