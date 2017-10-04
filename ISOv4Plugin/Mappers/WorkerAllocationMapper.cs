﻿/*
 * ISO standards can be purchased through the ANSI webstore at https://webstore.ansi.org
*/

using AgGateway.ADAPT.ISOv4Plugin.ExtensionMethods;
using AgGateway.ADAPT.ISOv4Plugin.ISOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgGateway.ADAPT.ApplicationDataModel.Logistics;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using AgGateway.ADAPT.ISOv4Plugin.ISOEnumerations;

namespace AgGateway.ADAPT.ISOv4Plugin.Mappers
{
    public interface IWorkerAllocationMapper
    {
        IEnumerable<ISOWorkerAllocation> ExportWorkerAllocations(IEnumerable<PersonRole> adaptWorkerAllocations);
        ISOWorkerAllocation ExportWorkerAllocation(PersonRole adaptWorkerAllocation);
        IEnumerable<PersonRole> ImportWorkerAllocations(IEnumerable<ISOWorkerAllocation> isoWorkerAllocations);
        PersonRole ImportWorkerAllocation(ISOWorkerAllocation isoWorkerAllocation);
    }

    public class WorkerAllocationMapper : BaseMapper, IWorkerAllocationMapper
    {
        public WorkerAllocationMapper(TaskDataMapper taskDataMapper) : base(taskDataMapper, "WAN")
        {
        }

        #region Export
        public IEnumerable<ISOWorkerAllocation> ExportWorkerAllocations(IEnumerable<PersonRole> adaptWorkerAllocations)
        {
            List <ISOWorkerAllocation> wans = new List<ISOWorkerAllocation>();
            foreach (PersonRole role in adaptWorkerAllocations)
            {
                ISOWorkerAllocation wan = ExportWorkerAllocation(role);
                wans.Add(wan);
            }
            return wans;
        }

        public ISOWorkerAllocation ExportWorkerAllocation(PersonRole adaptWorkerAllocation)
        {
            ISOWorkerAllocation wan = new ISOWorkerAllocation();

            //Worker ID
            if (TaskDataMapper.ISOIdMap.ContainsKey(adaptWorkerAllocation.PersonId))
            {
                wan.WorkerIdRef = TaskDataMapper.ISOIdMap[adaptWorkerAllocation.PersonId];
            }

            //Allocation Stamps
            if (adaptWorkerAllocation.TimeScopes.Any())
            {
                wan.AllocationStamp = AllocationStampMapper.ExportAllocationStamps(adaptWorkerAllocation.TimeScopes).FirstOrDefault();
            }

            return wan;
        }

        #endregion Export 

        #region Import

        public IEnumerable<PersonRole> ImportWorkerAllocations(IEnumerable<ISOWorkerAllocation> isoWorkerAllocations)
        {
            //Import WANs
            List<PersonRole> adaptWorkerAllocations = new List<PersonRole>();
            foreach (ISOWorkerAllocation isoWorkerAllocation in isoWorkerAllocations)
            {
                PersonRole adaptWorkerAllocation = ImportWorkerAllocation(isoWorkerAllocation);
                adaptWorkerAllocations.Add(adaptWorkerAllocation);
            }

            return adaptWorkerAllocations;
        }

        public PersonRole ImportWorkerAllocation(ISOWorkerAllocation isoWorkerAllocation)
        {
            PersonRole adaptWorkerAllocation = new PersonRole();

            //Worker ID
            if (TaskDataMapper.ADAPTIdMap.ContainsKey(isoWorkerAllocation.WorkerIdRef))
            {
                 adaptWorkerAllocation.PersonId = TaskDataMapper.ADAPTIdMap[isoWorkerAllocation.WorkerIdRef].Value;
            }

            //Allocation Stamps
            if (isoWorkerAllocation.AllocationStamp != null)
            {
                adaptWorkerAllocation.TimeScopes = AllocationStampMapper.ImportAllocationStamps(new List<ISOAllocationStamp>() { isoWorkerAllocation.AllocationStamp }).ToList();
            }

            return adaptWorkerAllocation;
        }
        #endregion Import
    }
}
