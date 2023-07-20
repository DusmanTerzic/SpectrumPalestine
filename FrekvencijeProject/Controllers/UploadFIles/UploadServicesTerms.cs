using FrekvencijeProject.JSON.Allocations;
using FrekvencijeProject.Models;
using FrekvencijeProject.Models.Allocation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Controllers.UploadFIles
{
    public class UploadServicesTerms
    {
        private readonly AllocationDBContext _conAll;
        
        public UploadServicesTerms(AllocationDBContext conAll)
        {
            _conAll = conAll;
        }

        
        public void AddPrimaryValues(DataSet dataSetDatabase)
        {
            string AllocationNameFirstLevel = "";
            string AllocationNameSecondLevel = "";
            string AllocationNameThirdLevel = "";
            foreach (DataRow dr in dataSetDatabase.Tables[0].Rows)
            {

                //Debug.WriteLine("first entrance");
                for (int i = 0; i < dataSetDatabase.Tables[0].Columns.Count; i++)
                {

                    if (dataSetDatabase.Tables[0].Columns[i].ColumnName == "Layer_1")
                    {

                        if (dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString() != "")
                        {
                            //Debug.WriteLine("second entrance");
                            string tempAllTerm = _conAll.AllocationTermDb
                            .OrderByDescending(p => p.AllocationTermId)
                            .Select(r => r.AllocationTermId)
                            .FirstOrDefault().ToString();

                            int value = 0;
                            if (tempAllTerm != null)
                            {
                                if (tempAllTerm != "")
                                {
                                    value = int.Parse(tempAllTerm);
                                }
                            }
                            value++;
                            // Debug.WriteLine("www:" + value);

                            var valueOr = (from all in _conAll.AllocationTermDb
                                           where all.name == dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString()
                                           && all._PRIMARY == true
                                           select new
                                           {
                                               all.AllocationTermId
                                           }).SingleOrDefault();


                            //foreach(var all in valueOr)
                            //{
                            //    //Debug.WriteLine("tttt:" + all.AllocationTermId);
                            //}

                            if (valueOr == null)
                            {
                                //Debug.WriteLine("qwer:");
                                AllocationTermDb allTerm = new AllocationTermDb();
                                allTerm.AllocationTermId = value;
                                allTerm.name = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                allTerm.Layer = 1;
                                allTerm._PRIMARY = true;
                                _conAll.AllocationTermDb.Add(allTerm);
                                _conAll.SaveChanges();
                                AllocationNameFirstLevel = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                //Debug.WriteLine("qwer:"+ AllocationNameFirstLevel);
                            }
                            else
                            {
                                AllocationNameFirstLevel = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                //Debug.WriteLine("www:" + value);
                            }

                        }
                        else
                        {
                            AllocationNameFirstLevel = "";
                        }

                    }
                    else if (dataSetDatabase.Tables[0].Columns[i].ColumnName == "Layer_2")
                    {
                        if (dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString() != "")
                        {
                            //takes id for allocation on first level.
                            var valueOrSecond = (from all in _conAll.AllocationTermDb
                                                 where all.name == AllocationNameFirstLevel && all._PRIMARY == true
                                                 select new
                                                 {
                                                     all.AllocationTermId
                                                 }).SingleOrDefault();


                            string tempAllTermSecond = _conAll.AllocationTermDb
                            .OrderByDescending(p => p.AllocationTermId)
                            .Select(r => r.AllocationTermId)
                            .FirstOrDefault().ToString();

                            int valueSecond = 0;
                            if (tempAllTermSecond != null)
                            {
                                if (tempAllTermSecond != "")
                                {
                                    valueSecond = int.Parse(tempAllTermSecond);
                                }
                            }
                            valueSecond++;
                            //before insert check is current name exist in table
                            var CurrentAll = (from all in _conAll.AllocationTermDb

                                              where all._PRIMARY == true
                                              select new
                                              {
                                                  all.AllocationTermId,
                                                  all.name
                                              }).ToList();
                            var IsExistSecond = CurrentAll.Where(x => x.name == dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString()).SingleOrDefault();

                            
                            if (IsExistSecond == null)
                            {
                                AllocationTermDb allTerm = new AllocationTermDb();
                                allTerm.AllocationTermId = valueSecond;
                                allTerm.name = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                allTerm.Layer = 2;
                                allTerm._PRIMARY = true;
                                allTerm.first_up_lvl_id = valueOrSecond.AllocationTermId;
                                _conAll.AllocationTermDb.Add(allTerm);
                                _conAll.SaveChanges();
                                AllocationNameSecondLevel = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                            }
                            else
                            {
                                AllocationNameSecondLevel = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                            }

                        }
                        else
                        {
                            AllocationNameSecondLevel = "";
                        }
                    }
                    else if (dataSetDatabase.Tables[0].Columns[i].ColumnName == "Layer_3")
                    {
                        if (dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString() != "")
                        {
                            int First = 0;
                            if (AllocationNameFirstLevel != "") { 
                                var valueOfSecond = (from all in _conAll.AllocationTermDb
                                                     where all.name == AllocationNameFirstLevel
                                                     && all._PRIMARY == true
                                                     select new
                                                     {
                                                         all.AllocationTermId
                                                     }).SingleOrDefault();
                                First = valueOfSecond.AllocationTermId;
                            }
                            else
                            {
                                string temp = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                var allocationArray = temp.Split(" (");
                                //Debug.WriteLine("value:" + allocationArray[0]);
                                var valueOfSecond = (from all in _conAll.AllocationTermDb
                                                     where all.name == allocationArray[0]
                                                     && all._PRIMARY == true && all.Layer == 1
                                                     select new
                                                     {
                                                         all.AllocationTermId
                                                     }).SingleOrDefault();
                                if (valueOfSecond != null)
                                {
                                    First = valueOfSecond.AllocationTermId;
                                }
                            }

                            //Debug.WriteLine("value is next:" + AllocationNameSecondLevel);

                            int Third = 0;
                            if (AllocationNameSecondLevel != "") { 
                                var valueOfThird = (from all in _conAll.AllocationTermDb
                                                    where all.name == AllocationNameSecondLevel
                                                    && all._PRIMARY == true
                                                    select new
                                                    {
                                                        all.AllocationTermId
                                                    }).SingleOrDefault();
                            Third = valueOfThird.AllocationTermId;
                           }
                            

                            string tempAllTermThird = _conAll.AllocationTermDb
                            .OrderByDescending(p => p.AllocationTermId)
                            .Select(r => r.AllocationTermId)
                            .FirstOrDefault().ToString();

                            int valueThird = 0;
                            if (tempAllTermThird != null)
                            {
                                if (tempAllTermThird != "")
                                {
                                    valueThird = int.Parse(tempAllTermThird);
                                }
                            }
                            valueThird++;

                            if (Third > 0 && First > 0)
                            {
                                var CurrentAll = (from all in _conAll.AllocationTermDb

                                                  where all._PRIMARY == true
                                                  select new
                                                  {
                                                      all.AllocationTermId,
                                                      all.name
                                                  }).ToList();
                                var WhereCurrentAll = CurrentAll.Where(x => x.name == dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString()).SingleOrDefault();

                                if (WhereCurrentAll == null)
                                {
                                    AllocationTermDb allTerm = new AllocationTermDb();
                                    allTerm.AllocationTermId = valueThird;
                                    allTerm.name = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                    allTerm.Layer = 3;
                                    allTerm._PRIMARY = true;
                                    allTerm.first_up_lvl_id = First;
                                    allTerm.second_up_lvl_id = Third;
                                    _conAll.AllocationTermDb.Add(allTerm);
                                    _conAll.SaveChanges();
                                }
                            }
                            else if(Third == 0 && First> 0)
                            {

                                string temp = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                var valueOfThird = (from all in _conAll.AllocationTermDb
                                                     where all.name == temp
                                                     && all._PRIMARY == true
                                                     select new
                                                     {
                                                         all.AllocationTermId
                                                     }).SingleOrDefault();
                                if (valueOfThird == null)
                                {
                                    AllocationTermDb allTerm = new AllocationTermDb();
                                    allTerm.AllocationTermId = valueThird;
                                    allTerm.name = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                    allTerm.Layer = 3;
                                    allTerm._PRIMARY = true;
                                    allTerm.first_up_lvl_id = First;

                                    _conAll.AllocationTermDb.Add(allTerm);
                                    _conAll.SaveChanges();
                                }
                            }
                            
                            //Debug.WriteLine("value 3:" + dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString());
                        }
                    }
                    //Debug.WriteLine(dataSetDatabase.Tables[0].Columns[i].ColumnName + "=====" + dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString());
                }
            }
        }

        public void AddSecondaryValues(DataSet dataSetDatabase)
        {
            string AllocationNameFirstLevel = "";
            string AllocationNameSecondLevel = "";
            string AllocationNameThirdLevel = "";
            //Debug.WriteLine("first entrance=" + dataSetDatabase.Tables[0].Rows.Count);
            foreach (DataRow dr in dataSetDatabase.Tables[0].Rows)
            {

                //Debug.WriteLine("first entrance");
                for (int i = 0; i < dataSetDatabase.Tables[0].Columns.Count; i++)
                {

                    if (dataSetDatabase.Tables[0].Columns[i].ColumnName == "Layer_1")
                    {

                        if (dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString() != "")
                        {
                            //Debug.WriteLine("second entrance");
                            string tempAllTerm = _conAll.AllocationTermDb
                            .OrderByDescending(p => p.AllocationTermId)
                            .Select(r => r.AllocationTermId)
                            .FirstOrDefault().ToString();

                            int value = 0;
                            if (tempAllTerm != null)
                            {
                                if (tempAllTerm != "")
                                {
                                    value = int.Parse(tempAllTerm);
                                }
                            }
                            value++;
                           // Debug.WriteLine("www:" + value);

                            var valueOr = (from all in _conAll.AllocationTermDb
                                           where all.name == dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString()
                                           && all._PRIMARY == false
                                           select new
                                           {
                                               all.AllocationTermId
                                           }).SingleOrDefault();


                            //foreach(var all in valueOr)
                            //{
                            //    //Debug.WriteLine("tttt:" + all.AllocationTermId);
                            //}

                            if (valueOr == null)
                            {
                                //Debug.WriteLine("qwer:");
                                AllocationTermDb allTerm = new AllocationTermDb();
                                allTerm.AllocationTermId = value;
                                allTerm.name = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                allTerm.Layer = 1;
                                allTerm._PRIMARY = false;
                                _conAll.AllocationTermDb.Add(allTerm);
                                _conAll.SaveChanges();
                                AllocationNameFirstLevel = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                //Debug.WriteLine("qwer:"+ AllocationNameFirstLevel);
                            }
                            else
                            {
                                AllocationNameFirstLevel = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                //Debug.WriteLine("www:" + value);
                            }

                        }
                        else
                        {
                            AllocationNameFirstLevel = "";
                        }

                    }
                    else if (dataSetDatabase.Tables[0].Columns[i].ColumnName == "Layer_2")
                    {
                        if (dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString() != "")
                        {
                            var valueOrSecond = (from all in _conAll.AllocationTermDb
                                                 where all.name == AllocationNameFirstLevel && all._PRIMARY == false
                                                 select new
                                                 {
                                                     all.AllocationTermId
                                                 }).SingleOrDefault();


                            string tempAllTermSecond = _conAll.AllocationTermDb
                            .OrderByDescending(p => p.AllocationTermId)
                            .Select(r => r.AllocationTermId)
                            .FirstOrDefault().ToString();

                            int valueSecond = 0;
                            if (tempAllTermSecond != null)
                            {
                                if (tempAllTermSecond != "")
                                {
                                    valueSecond = int.Parse(tempAllTermSecond);
                                }
                            }
                            valueSecond++;
                            //before insert check is current name exist in table
                            var CurrentAll = (from all in _conAll.AllocationTermDb

                                              where all._PRIMARY == false
                                              select new
                                              {
                                                  all.AllocationTermId,
                                                  all.name
                                              }).ToList();
                            var IsExistSecond = CurrentAll.Where(x => x.name == dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString()).SingleOrDefault();


                            //var IsExistSecond = (from all in _conAll.AllocationTermDb
                            //                     where all.name == dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString()
                            //                     && all._PRIMARY == false
                            //                     select new
                            //                     {
                            //                         all.AllocationTermId
                            //                     }).SingleOrDefault();
                            if (IsExistSecond == null)
                            {
                                AllocationTermDb allTerm = new AllocationTermDb();
                                allTerm.AllocationTermId = valueSecond;
                                allTerm.name = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                allTerm.Layer = 2;
                                allTerm._PRIMARY = false;
                                allTerm.first_up_lvl_id = valueOrSecond.AllocationTermId;
                                _conAll.AllocationTermDb.Add(allTerm);
                                _conAll.SaveChanges();
                                AllocationNameSecondLevel = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                            }
                            else
                            {
                                AllocationNameSecondLevel = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                            }

                        }
                        else
                        {
                            AllocationNameSecondLevel = "";
                        }
                    }
                    else if (dataSetDatabase.Tables[0].Columns[i].ColumnName == "Layer_3")
                    {
                        if (dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString() != "")
                        {
                            int First = 0;
                            if (AllocationNameFirstLevel != "")
                            {
                                var valueOfSecond = (from all in _conAll.AllocationTermDb
                                                     where all.name == AllocationNameFirstLevel
                                                     && all._PRIMARY == false
                                                     select new
                                                     {
                                                         all.AllocationTermId
                                                     }).SingleOrDefault();
                                First = valueOfSecond.AllocationTermId;
                            }
                            else
                            {
                                string temp = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                var allocationArray = temp.Split(" (");
                                //Debug.WriteLine("value:" + allocationArray[0]);
                                var valueOfSecond = (from all in _conAll.AllocationTermDb
                                                     where all.name == allocationArray[0]
                                                     && all._PRIMARY == false && all.Layer == 1
                                                     select new
                                                     {
                                                         all.AllocationTermId
                                                     }).SingleOrDefault();
                                if (valueOfSecond != null)
                                {
                                    First = valueOfSecond.AllocationTermId;
                                }
                            }

                            //Debug.WriteLine("value is next:" + AllocationNameSecondLevel);

                            int Third = 0;
                            if (AllocationNameSecondLevel != "")
                            {
                                var valueOfThird = (from all in _conAll.AllocationTermDb
                                                    where all.name == AllocationNameSecondLevel
                                                    && all._PRIMARY == false
                                                    select new
                                                    {
                                                        all.AllocationTermId
                                                    }).SingleOrDefault();
                                Third = valueOfThird.AllocationTermId;
                            }


                            string tempAllTermThird = _conAll.AllocationTermDb
                            .OrderByDescending(p => p.AllocationTermId)
                            .Select(r => r.AllocationTermId)
                            .FirstOrDefault().ToString();

                            int valueThird = 0;
                            if (tempAllTermThird != null)
                            {
                                if (tempAllTermThird != "")
                                {
                                    valueThird = int.Parse(tempAllTermThird);
                                }
                            }
                            valueThird++;

                            if (Third > 0 && First > 0)
                            {
                                //Debug.WriteLine("third false:" + dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString());
                                var CurrentAll = (from all in _conAll.AllocationTermDb

                                                  where all._PRIMARY == false
                                                  select new
                                                  {
                                                      all.AllocationTermId,
                                                      all.name
                                                  }).ToList();
                                var WhereCurrentAll  = CurrentAll.Where(x => x.name == dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString()).SingleOrDefault();
                                
                                if (WhereCurrentAll == null)
                                {
                                    AllocationTermDb allTerm = new AllocationTermDb();
                                    allTerm.AllocationTermId = valueThird;
                                    allTerm.name = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                    allTerm.Layer = 3;
                                    allTerm._PRIMARY = false;
                                    allTerm.first_up_lvl_id = First;
                                    allTerm.second_up_lvl_id = Third;
                                    _conAll.AllocationTermDb.Add(allTerm);
                                    _conAll.SaveChanges();
                                }
                                //AllocationTermDb allTerm = new AllocationTermDb();
                                //allTerm.AllocationTermId = valueThird;
                                //allTerm.name = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                //allTerm.Layer = 3;
                                //allTerm._PRIMARY = false;
                                //allTerm.first_up_lvl_id = First;
                                //allTerm.second_up_lvl_id = Third;
                                //_conAll.AllocationTermDb.Add(allTerm);
                                //_conAll.SaveChanges();
                            }
                            else if (Third == 0 && First > 0)
                            {

                                string temp = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                var valueOfThird = (from all in _conAll.AllocationTermDb
                                                    where all.name == temp
                                                    && all._PRIMARY == false
                                                    select new
                                                    {
                                                        all.AllocationTermId
                                                    }).SingleOrDefault();
                                if (valueOfThird == null)
                                {
                                    AllocationTermDb allTerm = new AllocationTermDb();
                                    allTerm.AllocationTermId = valueThird;
                                    allTerm.name = dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString();
                                    allTerm.Layer = 3;
                                    allTerm._PRIMARY = false;
                                    allTerm.first_up_lvl_id = First;

                                    _conAll.AllocationTermDb.Add(allTerm);
                                    _conAll.SaveChanges();
                                }
                            }
                            //Debug.WriteLine("value 3:" + dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString());
                        }
                    }
                    //Debug.WriteLine(dataSetDatabase.Tables[0].Columns[i].ColumnName + "=====" + dr[dataSetDatabase.Tables[0].Columns[i].ColumnName].ToString());
                }
            }
        }


        
    }
}
