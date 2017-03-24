﻿using System.Collections.Generic;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Persistence.Data;
using EcsRx.Pools;
using EcsRx.Unity.Components;
using EcsRx.Unity.MonoBehaviours.Helpers;
using EcsRx.Unity.MonoBehaviours.Models;
using UnityEngine;
using Zenject;

namespace EcsRx.Unity.MonoBehaviours
{
    public class EntityBinding : MonoBehaviour
    {
        [Inject]
        public IPoolManager PoolManager { get; private set; }

        [SerializeField]
        public string PoolName;

        [SerializeField]
        public List<ComponentData> ComponentCache = new List<ComponentData>();

        [Inject]
        public void RegisterEntity()
        {
            if (!gameObject.activeInHierarchy || !gameObject.activeSelf) { return; }

            var poolToUse = GetPool();
            var createdEntity = poolToUse.CreateEntity();
            createdEntity.AddComponent(new ViewComponent { View = gameObject });
            SetupEntityBinding(createdEntity, poolToUse);
            SetupEntityComponents(createdEntity);

            Destroy(this);
        }

        private void SetupEntityBinding(IEntity entity, IPool pool)
        {
            var entityBinding = gameObject.AddComponent<EntityView>();
            entityBinding.Entity = entity;
            entityBinding.Pool = pool;
        }

        private void SetupEntityComponents(IEntity entity)
        {
            for (var i = 0; i < ComponentCache.Count; i++)
            {
                var component = EntityTransformer.DeserializeComponent(ComponentCache[i]);
                entity.AddComponent(component);
            }
        }

        public IPool GetPool()
        {
            if (string.IsNullOrEmpty(PoolName))
            { return PoolManager.GetPool(); }

            if (PoolManager.Pools.All(x => x.Name != PoolName))
            { return PoolManager.CreatePool(PoolName); }

            return PoolManager.GetPool(PoolName);
        }
    }
}