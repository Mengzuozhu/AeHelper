// Copyright 2010 ESRI
// 
// All rights reserved under the copyright laws of the United States
// and applicable international laws, treaties, and conventions.
// 
// You may freely redistribute and use this sample code, with or
// without modification, provided you include the original copyright
// notice and use restrictions.
// 
// See the use restrictions.
// 

using System;
using System.Collections;
using ESRI.ArcGIS.Carto;

namespace AeDisplay.MapAndPageLayoutSynchApp
{
    /// <summary>
    /// Implementation of interface IMaps which is eventually a collection of Maps
    /// </summary>
    public class Maps : IMaps, IDisposable
    {
        //class member - using internally an ArrayList to manage the Maps collection
        private ArrayList m_array;

        #region class constructor

        /// <summary>
        /// 地图
        /// </summary>
        public Maps()
        {
            m_array = new ArrayList();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose the collection
        /// </summary>
        public void Dispose()
        {
            if (m_array == null) return;
            m_array.Clear();
            m_array = null;
        }

        #endregion

        #region IMaps Members

        /// <summary>
        /// Remove the map at the given index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if (index > m_array.Count || index < 0)
            {
                throw new Exception("Maps::RemoveAt:\r\nIndex is out of range!");
            }
            m_array.RemoveAt(index);
        }

        /// <summary>
        /// Reset the Maps array
        /// </summary>
        public void Reset()
        {
            m_array.Clear();
        }

        /// <summary>
        /// Get the number of Maps in the collection
        /// </summary>
        public int Count
        {
            get
            {
                return m_array.Count;
            }
        }

        /// <summary>
        /// Return the map at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IMap get_Item(int index)
        {
            if (index > m_array.Count || index < 0)
            {
                throw new Exception("Maps::get_Item:\r\nIndex is out of range!");
            }
            return m_array[index] as IMap;
        }

        /// <summary>
        /// Remove the instance of the given map
        /// </summary>
        /// <param name="map"></param>
        public void Remove(IMap map)
        {
            m_array.Remove(map);
        }

        /// <summary>
        /// Create a new map, add it to the collection and return it to the caller
        /// </summary>
        /// <returns></returns>
        public IMap Create()
        {
            IMap newMap = new MapClass();
            m_array.Add(newMap);
            return newMap;
        }

        /// <summary>
        /// Add the given map to the collection
        /// </summary>
        /// <param name="map"></param>
        public void Add(IMap map)
        {
            if (map == null)
            {
                throw new Exception("Maps::Add:\r\nNew map is mot initialized!");
            }
            m_array.Add(map);
        }

        #endregion
    }
}