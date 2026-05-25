using System;
using System.Collections.Generic;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;

namespace Styx.Logic.AreaManagement
{
	public class AreaManager
	{
		private readonly List<Area> _areas = new List<Area>();
		private GrindArea? _currentGrindArea;

		/// <summary>
		/// FEAT-41: Sets the current active area by list index.
		/// If the area has a GrindArea, sets it as the current grind area.
		/// </summary>
		public void SetAreaByIndex(int index)
		{
			if (index < 0 || index >= _areas.Count)
			{
				throw new IndexOutOfRangeException("Index out of range for area list.");
			}

			Area area = _areas[index];
			if (area is GrindArea grindArea)
			{
				_currentGrindArea = grindArea;
			}
		}

		public void SetArea(GrindArea? area)
		{
			_currentGrindArea = area;
		}

		public void Add(Area value)
		{
			_areas.Add(value);
		}

		public GrindArea? CurrentGrindArea
		{
			get
			{
				GrindArea? area = _currentGrindArea;
				if (area == null)
				{
					if (ProfileManager.CurrentProfile != null)
					{
						return ProfileManager.CurrentProfile.GrindArea;
					}
					return null;
				}
				return area;
			}
		}

		public AreaManager()
		{
		}
	}
}
