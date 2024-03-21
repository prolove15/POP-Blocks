using System;
using UnityEngine;

namespace Malee.List {

	public class ReorderableAttribute : PropertyAttribute {

		public bool add;
		public bool remove;
		public bool draggable;
		public bool singleLine;
		public bool paginate;
		public bool sortable;
		public bool labels;
		public bool expandByDefault;
		public int pageSize;
		public string elementNameProperty;
		public string elementNameOverride;
		public string elementIconPath;
		public Type surrogateType;
		public string surrogateProperty;

		public ReorderableAttribute()
			: this(null) {
		}

		public ReorderableAttribute(string elementNameProperty)
			: this(true, true, true, false, elementNameProperty, null, null) {
		}

		public ReorderableAttribute(string elementNameProperty, string elementIconPath)
			: this(true, true, true, false, elementNameProperty, null, elementIconPath) {
		}

		public ReorderableAttribute(string elementNameProperty, string elementNameOverride, string elementIconPath)
			: this(true, true, true, false, elementNameProperty, elementNameOverride, elementIconPath) {
		}

		public ReorderableAttribute(bool add, bool remove, bool draggable,bool expandByDefault, string elementNameProperty = null, string elementIconPath = null) 
			: this(add, remove, draggable, expandByDefault, elementNameProperty, null, elementIconPath) {
		}

		public ReorderableAttribute(bool add, bool remove, bool draggable, bool expandByDefault, string elementNameProperty = null, string elementNameOverride = null, string elementIconPath = null) {

			this.add = add;
			this.remove = remove;
			this.draggable = draggable;
			this.elementNameProperty = elementNameProperty;
			this.elementNameOverride = elementNameOverride;
			this.elementIconPath = elementIconPath;
			this.expandByDefault = expandByDefault;
			sortable = true;
			labels = true;
		}
	}
}
