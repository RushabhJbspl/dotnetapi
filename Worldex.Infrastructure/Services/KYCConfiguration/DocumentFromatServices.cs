using Worldex.Core.Entities.KYCConfiguration;
using Worldex.Core.Interfaces.KYCConfiguration;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.KYCConfiguration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.KYCConfiguration
{
    public class DocumentFromatServices : IDocumentMaster
    {
        private readonly WorldexContext _dbContext;

        private readonly ICustomExtendedRepository<DocumentMaster> _customExtendedRepository;
        public DocumentFromatServices(WorldexContext context, ICustomExtendedRepository<DocumentMaster> customExtendedRepository)
        {
            _customExtendedRepository = customExtendedRepository;
            _dbContext = context;
        }
        public Guid AddDocumentMaster(DocumentmasterReqViewMode documentmasterReqViewMode)
        {
            try
            {
                DocumentMaster documentMaster = new DocumentMaster()
                {
                    Name = documentmasterReqViewMode.Name,
                    CreatedBy = documentmasterReqViewMode.UserId,
                    CreatedDate = DateTime.UtcNow
                };
                _customExtendedRepository.Insert(documentMaster);
                return documentMaster.Id;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        public Guid IsDocumentExist(string Name)
        {
            try
            {
                var IsDocumentmasterExist = _customExtendedRepository.Table.FirstOrDefault(i => i.Name == Name);
                if (IsDocumentmasterExist == null)
                {
                    return Guid.Empty;
                }
                else
                {
                    return IsDocumentmasterExist.Id;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }

        }

        public List<KYCDocumentConfigurationListDisplayViewmodel> KYCConfigurationUserWiseDocumentList(int Userid)
        {
            try
            {
                IQueryable<KYCDocumentConfigurationListViewmodel> Result;
                IQueryable<KYCDocumentKYCFormatListViewmodel> DocumentFormat;
                KYCDocumentConfigurationListDisplayViewmodel kYCDocumentConfigurationListDisplayViewmodel = new KYCDocumentConfigurationListDisplayViewmodel();
                List<KYCDocumentConfigurationListDisplayViewmodel> kYCDocumentConfigurationListDisplayListViewmodel = new List<KYCDocumentConfigurationListDisplayViewmodel>();
                string Document = string.Empty;
                string DocumentFind = string.Empty;
                string DocumentFormatData = string.Empty;
                string Query = @"Select KICM.id,BU.Id As UserID,BU.UserName,KLM.Id As KYCLevel,KID.Name As KYCDocument,KICM.Status,KID.DocumentMasterId From BizUser As BU  ";
                Query += "Inner Join kYCIdentityConfigurationMapping As KICM On BU.Id=KICM.Userid ";
                Query += "Inner Join kYCIdentityMaster As KID   On KICM.KYCConfigurationMasterId=KID.Id ";
                Query += "Inner join kYCLevelMaster KLM On KLM.Id=KICM.LevelId  Where Bu.Id= " + Userid;
                Result = _dbContext.KYCDocumentConfigurationLists.FromSql(Query);

                string[] DocumentId;
                foreach (var item in Result)
                {
                    Document = item.DocumentMasterId;
                    DocumentId = Document.Split(',');

                    foreach (string DocumentCollection in DocumentId)
                    {
                        DocumentFind += "'" + DocumentCollection + "',";
                    }
                    DocumentFind = DocumentFind.Remove(DocumentFind.Length - 1);


                    Query = string.Empty;
                    Query = "Select  Name as DocumentName from DocumentMaster where Id in (" + DocumentFind + ")";
                    DocumentFormat = _dbContext.KYCDocumentKYCFormatListViewmodels.FromSql(Query);
                    foreach (var DocumentData in DocumentFormat)
                    {
                        DocumentFormatData += DocumentData.DocumentName + "-";
                    }
                    DocumentFormatData = DocumentFormatData.Remove(DocumentFormatData.Length - 1);
                    kYCDocumentConfigurationListDisplayViewmodel.ID = item.ID;
                    kYCDocumentConfigurationListDisplayViewmodel.KYCDocument = item.KYCDocument;
                    kYCDocumentConfigurationListDisplayViewmodel.KYCLevel = item.KYCLevel;
                    kYCDocumentConfigurationListDisplayViewmodel.Status = item.Status;
                    kYCDocumentConfigurationListDisplayViewmodel.DocumentName = DocumentFormatData;
                    kYCDocumentConfigurationListDisplayViewmodel.DocumentMasterId = item.DocumentMasterId;
                    kYCDocumentConfigurationListDisplayViewmodel.UserName = item.UserName;
                    kYCDocumentConfigurationListDisplayViewmodel.UserID = item.UserID;

                    kYCDocumentConfigurationListDisplayListViewmodel.Add(kYCDocumentConfigurationListDisplayViewmodel);
                }
                return kYCDocumentConfigurationListDisplayListViewmodel;





            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        public Guid UpdateDocumentMaster(DocumentMasterUpdateReqViewModel documentMasterUpdateViewModel)
        {
            try
            {
                var IsDocumentmasterExist = _customExtendedRepository.Table.FirstOrDefault(i => i.Id == documentMasterUpdateViewModel.Id);
                if (IsDocumentmasterExist != null)
                {
                    IsDocumentmasterExist.Name = documentMasterUpdateViewModel.Name;
                    IsDocumentmasterExist.Status = documentMasterUpdateViewModel.Status;
                    IsDocumentmasterExist.UpdatedBy = documentMasterUpdateViewModel.UserId;
                    IsDocumentmasterExist.UpdatedDate = DateTime.UtcNow;

                    _customExtendedRepository.Update(IsDocumentmasterExist);
                    return IsDocumentmasterExist.Id;
                }
                else
                {
                    return Guid.Empty;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public List<DocumentMasterListViewModel> KYCDocumentGetList()
        {
            try
            {
                IQueryable<DocumentMasterListViewModel> Result;

                string Query = @"Select Id, Name,Status,CreatedDate from DocumentMaster";
                Result = _dbContext.DocumentMasterListViewModels.FromSql(Query);
                return Result.ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// This Method are check the Document format and check it format is valide or not
        /// </summary>
        /// <returns></returns>


        public List<KYCDocumentKYCFormatListViewmodel> GetFormatList(string DocumentIddata)
        {
            try
            {
                if (!string.IsNullOrEmpty(DocumentIddata))
                {
                    IQueryable<KYCDocumentKYCFormatListViewmodel> DocumentFormat;
                    string DocumentIdSplite = string.Empty;
                    string DocumentFormatData = string.Empty;
                    string[] DocumentId;
                    DocumentId = DocumentIddata.Split(',');
                    foreach (string DocumentCollection in DocumentId)
                    {
                        DocumentIdSplite += "'" + DocumentCollection + "',";
                    }
                    DocumentIdSplite = DocumentIdSplite.Remove(DocumentIdSplite.Length - 1);
                    string Query = @"Select name as DocumentName From documentmaster where id in (" + DocumentIdSplite + ")";
                    DocumentFormat = _dbContext.KYCDocumentKYCFormatListViewmodels.FromSql(Query);
                    return DocumentFormat.ToList();
                    //foreach (var item in DocumentFormat)
                    //{
                    //    DocumentFormatData += item.DocumentName + ",";
                    //}
                    //DocumentFormatData = DocumentFormatData.Remove(DocumentFormatData.Length - 1);
                    //return DocumentFormatData;

                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {

                ex.ToString();
                throw ex;
                
            }
        }


    }
}
