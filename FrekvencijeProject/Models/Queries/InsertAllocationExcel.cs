using FrekvencijeProject.JSON.Allocations;
using FrekvencijeProject.Models.AllocationTerms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Queries
{
    public class InsertAllocationExcel
    {
        private AllocationDBContext _conAll;
        //this class will be used to insert allocation from excel

        public InsertAllocationExcel(AllocationDBContext conAll)
        {
            _conAll = conAll;
        }
        //method for inserting allocation
        public void InsertNewOne(string tempVa,int AllocationRangeId)
        {
            bool isprimary = false;
            string allocation = "";
            //cut allocation where contains parentheses so in that way easily change is primary or secondary allocation.
            //example:
            //value:EARTH EXPLORATION-SATELLITE (passive)==True==EARTH EXPLORATION-SATELLITE 
            
            allocation = tempVa;
            
            //Debug.WriteLine("value:" + tempVa + "==" + isprimary+"=="+ allocation);
            string www = "";
            string tempwww = "";
            if (char.IsWhiteSpace(allocation[0]))
            {
                www = allocation.Substring(1);
            }
           
            else
            {
                www = allocation;
            }
            char lastCharacter = www[www.Length - 1];
            if (char.IsWhiteSpace(lastCharacter))
            {
                tempwww = www.Remove(www.Length - 1, 1);
                //Debug.WriteLine("qqqqq:" + tempwww);
                www = tempwww;
                //Debug.WriteLine("xxxxx:" + www);
            }
            //Debug.WriteLine("new:" + www);
            //method IsAllUpper decide is current string with upper letters or not.
           
            
            //linq query to take AllocationTermId acording to allocation name.
            var queryTerm
                = (from all in _conAll.AllocationTermDb
                   
                   select new 
                   {
                       all.AllocationTermId,
                       all.name,
                       all._PRIMARY
                   }
             ).ToList();
            
                var NewQueryTerm = queryTerm.Where(a => a.name == www ).SingleOrDefault();
            var QueryAllocation
                = (from all in _conAll.AllocationDb
                   where all.AllocationRangeId == AllocationRangeId && all.AllocationTermId == NewQueryTerm.AllocationTermId
                   select new 
                   {
                       all.AllocationId
                   }
             ).SingleOrDefault();
            if (QueryAllocation == null)
            {
                //insert current allocation.
                AllocationDb tempAll = new AllocationDb();
                tempAll.AllocationTermId = NewQueryTerm.AllocationTermId;
                tempAll.primary = NewQueryTerm._PRIMARY;
                tempAll.AllocationRangeId = AllocationRangeId;
                _conAll.AllocationDb.Add(tempAll);
                _conAll.SaveChanges();
            }
        }

        public void InsertFootnoteAllocation(string tempVa, int AllocationRangeId,string footnote)
        {
            string www = "";
            string tempFoot = "";
            bool isprimary = false;
            if (char.IsWhiteSpace(tempVa[0]))
            {
                www = tempVa.Substring(1);
            }
            else
            {
                www = tempVa;
            }
            char lastCharacter = www[www.Length - 1];
            //if string contains white space on the last character.
            if (char.IsWhiteSpace(lastCharacter))
            {
                string tempwww = www.Remove(www.Length - 1, 1);
               
                www = tempwww;
              
            }


            if (char.IsWhiteSpace(footnote[0]))
            {
                tempFoot = footnote.Substring(1);
            }
            else
            {
                tempFoot = footnote;
            }

            var queryTerm
            = (from all in _conAll.AllocationTermDb
               
               select new TempAllocationTerm
               {
                   AllocationTermId = all.AllocationTermId,
                   name = all.name,
                   _PRIMARY = all._PRIMARY
               }
              ).ToList();
            //Debug.WriteLine("test:" + www);
            //IEnumerable<a> NewQueryTerm = Enumerable.Empty<a>();
            TempAllocationTerm NewQueryTerm = null;
            //Debug.WriteLine("");
           //ovdje negdje problem
            NewQueryTerm = queryTerm.Where(x => x.name == www ).SingleOrDefault();
               
          
            var AllocationId = (from allVal in _conAll.AllocationDb
                                where allVal.AllocationTermId.Equals(NewQueryTerm.AllocationTermId)
                                && allVal.AllocationRangeId.Equals(AllocationRangeId)
                                select new
                                {
                                    allVal.AllocationId
                                }
                                   ).FirstOrDefault();


            var FootnoteId = (from allVal in _conAll.Footnote_description
                                where allVal.name.Equals(footnote)
                                
                                select new
                                {
                                    allVal.id_foot_desc
                                }
                               ).FirstOrDefault();
            var FootnoteAll = (from allVal in _conAll.FootnoteAllocation
                               join e in _conAll.AllocationDb on allVal.AllocationId equals e.AllocationId
                               where e.AllocationRangeId == AllocationRangeId &&
                                    allVal.FootDescId == FootnoteId.id_foot_desc && allVal.name == www
                               select new
                               {
                                   allVal.FootnoteId
                               }
                             ).FirstOrDefault();
            if (FootnoteAll == null)
            {

                FootnoteAllocation foot = new FootnoteAllocation();
                foot.id = 1;
                foot.name = tempFoot;
                foot.AllocationId = AllocationId.AllocationId;
                foot.FootDescId = FootnoteId.id_foot_desc;
                foot.isBand = true;
                _conAll.FootnoteAllocation.Add(foot);
                _conAll.SaveChanges();
            }
        }

        public void InsertFootnoteBandAllocation(int AllocationRangeId, string footnote,int AllocationTermId)
        {
            string www = "";
            if (char.IsWhiteSpace(footnote[0]))
            {
                www = footnote.Substring(1);
            }
            else
            {
                www = footnote;
            }

            if(www == "5.547")
            {
                www = "5.5470";
            }

            var AllocationId = (from allVal in _conAll.AllocationDb
                                where allVal.AllocationTermId.Equals(AllocationTermId)
                                && allVal.AllocationRangeId.Equals(AllocationRangeId)
                                select new
                                {
                                    allVal.AllocationId
                                }
                               ).FirstOrDefault();


            var FootnoteId = (from allVal in _conAll.Footnote_description
                              where allVal.name.Equals(www)

                              select new
                              {
                                  allVal.id_foot_desc
                              }
                              ).FirstOrDefault();

            var FootnoteAll = (from allVal in _conAll.FootnoteAllocation
                               join e in _conAll.AllocationDb on allVal.AllocationId equals e.AllocationId
                               where e.AllocationRangeId == AllocationRangeId &&
                                    allVal.FootDescId == FootnoteId.id_foot_desc && allVal.name == www
                               select new
                               {
                                   allVal.FootnoteId
                               }
                             ).FirstOrDefault();
            if (FootnoteAll == null)
            {

                //Debug.WriteLine("im entered 3");
                FootnoteAllocation foot = new FootnoteAllocation();
                foot.id = 3;
                foot.name = www;
                foot.AllocationId = AllocationId.AllocationId;
                foot.FootDescId = FootnoteId.id_foot_desc;
                foot.isBand = false;
                _conAll.FootnoteAllocation.Add(foot);
                _conAll.SaveChanges();
            }
            else
            {
                //Debug.WriteLine("this is already exist 3:" + AllocationRangeId + "::" + footnote + "==" + AllocationTermId);

            }

        }

        public void InsertFootnoteBandAllocation(int AllocationRangeId, string footnote, string AllocationTerm)
        {
            string www = "";
            string tempAll = "";
            bool isprimary = false;
            if (char.IsWhiteSpace(footnote[0]))
            {
                www = footnote.Substring(1);
            }
            else
            {
                www = footnote;
            }

            if (char.IsWhiteSpace(AllocationTerm[0]))
            {
                tempAll = AllocationTerm.Substring(1);
            }
            else
            {
                tempAll = AllocationTerm;
            }

            if (www == "5.547")
            {
                www = "5.5470";
            }
            //Debug.WriteLine("rrrr" + tempAll);

            char lastCharacter = tempAll[tempAll.Length - 1];
            if (char.IsWhiteSpace(lastCharacter))
            {
                string tempwww = tempAll.Remove(tempAll.Length - 1, 1);
                //Debug.WriteLine("qqqqq:" + tempwww);
                tempAll = tempwww;
                //Debug.WriteLine("xxxxx:" + www);
            }

            var queryTerm
           = (from all in _conAll.AllocationTermDb
              
              select new TempAllocationTerm
              {
                  AllocationTermId = all.AllocationTermId,
                  name = all.name,
                  _PRIMARY = all._PRIMARY
              }
             ).ToList();
            TempAllocationTerm NewQUeryTerm = null;
           
                NewQUeryTerm = queryTerm.Where(x => x.name == tempAll ).SingleOrDefault();
            

            //Debug.WriteLine("Test:" + tempAll+"===footnote:"+ www+ "===AllocationRangeId===" + AllocationRangeId);
            var AllocationId = (from allVal in _conAll.AllocationDb
                                where allVal.AllocationTermId.Equals(NewQUeryTerm.AllocationTermId)
                                && allVal.AllocationRangeId.Equals(AllocationRangeId)
                                select new
                                {
                                    allVal.AllocationId
                                }
                               ).FirstOrDefault();


            var FootnoteId = (from allVal in _conAll.Footnote_description
                              where allVal.name.Equals(www)

                              select new
                              {
                                  allVal.id_foot_desc
                              }
                              ).FirstOrDefault();

            var FootnoteAll = (from allVal in _conAll.FootnoteAllocation
                               join e in _conAll.AllocationDb on allVal.AllocationId equals e.AllocationId
                               where e.AllocationRangeId == AllocationRangeId &&
                                    allVal.FootDescId == FootnoteId.id_foot_desc && allVal.name == www
                               select new
                               {
                                   allVal.FootnoteId
                               }
                             ).FirstOrDefault();
            if (FootnoteAll == null)
            {
                //Debug.WriteLine("im entered");
                FootnoteAllocation foot = new FootnoteAllocation();
                foot.id = 4;
                foot.name = www;
                foot.AllocationId = AllocationId.AllocationId;
                foot.FootDescId = FootnoteId.id_foot_desc;
                foot.isBand = false;
                _conAll.FootnoteAllocation.Add(foot);
                _conAll.SaveChanges();
            }
            else
            {
                //Debug.WriteLine("this is already exist"+ AllocationRangeId+"::"+ footnote+"=="+AllocationTerm);

            }

        }


        public void InsertFootnoteAllocation2(string tempVa, int AllocationRangeId, string footnote)
        {
            string www = "";
            bool isprimary = false;
            if (char.IsWhiteSpace(tempVa[0]))
            {
                www = tempVa.Substring(1);
            }
            else
            {
                www = tempVa;
            }

            char lastCharacter = www[www.Length - 1];
            if (char.IsWhiteSpace(lastCharacter))
            {
                string tempwww = www.Remove(www.Length - 1, 1);
                //Debug.WriteLine("qqqqq:" + tempwww);
                www = tempwww;
                //Debug.WriteLine("xxxxx:" + www);
            }


            string wwwFoot = "";
            if (char.IsWhiteSpace(footnote[0]))
            {
                wwwFoot = footnote.Substring(1);
            }
            else
            {
                wwwFoot = footnote;
            }

            if (wwwFoot == "5.547")
            {
                wwwFoot = "5.5470";
            }

            if (IsAllUpper(www))
            {
                isprimary = true;
            }
            else
            {
                isprimary = false;
            }

            var queryTerm
           = (from all in _conAll.AllocationTermDb

              select new TempAllocationTerm
              {
                  AllocationTermId = all.AllocationTermId,
                  name = all.name,
                  _PRIMARY = all._PRIMARY
              }
             ).ToList();
            TempAllocationTerm NewQUeryTerm = null;
            NewQUeryTerm = queryTerm.Where(x => x.name == www ).SingleOrDefault();
            //Debug.WriteLine("Test 22:" + www + "===footnote:" + www + "===AllocationRangeId===" + AllocationRangeId);
            var AllocationId = (from allVal in _conAll.AllocationDb
                                where allVal.AllocationTermId.Equals(NewQUeryTerm.AllocationTermId)
                                && allVal.AllocationRangeId.Equals(AllocationRangeId)
                                select new
                                {
                                    allVal.AllocationId
                                }
                                ).FirstOrDefault();


            var FootnoteId = (from allVal in _conAll.Footnote_description
                              where allVal.name.Equals(wwwFoot)

                              select new
                              {
                                  allVal.id_foot_desc
                              }
                               ).FirstOrDefault();
            var FootnoteAll = (from allVal in _conAll.FootnoteAllocation
                               join e in _conAll.AllocationDb on allVal.AllocationId equals e.AllocationId
                               where e.AllocationRangeId == AllocationRangeId &&
                                    allVal.FootDescId == FootnoteId.id_foot_desc && allVal.name == www
                               select new
                               {
                                   allVal.FootnoteId
                               }
                             ).FirstOrDefault();

            if (FootnoteAll == null)
            {
                FootnoteAllocation foot = new FootnoteAllocation();
                foot.id = 2;
                foot.name = wwwFoot;
                foot.AllocationId = AllocationId.AllocationId;
                foot.FootDescId = FootnoteId.id_foot_desc;
                foot.isBand = true;
                _conAll.FootnoteAllocation.Add(foot);
                _conAll.SaveChanges();
            }
        }

        bool IsAllUpper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i].Equals('-'))
                {
                    continue;
                }
                else if (input[i].Equals(' '))
                {
                    continue;
                }
                if (!Char.IsUpper(input[i]))
                    return false;
            }

            return true;
        }
    }
}
