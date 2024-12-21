using Microsoft.EntityFrameworkCore;
using news.application.Contracts.Interfaces;
using news.application.DomainModelDTOs;
using news.application.DomainModelDTOs.FilterDTOs;
using news.application.Framework;
using news.domain.Models;
using news.infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace news.infrastructure.Repositories
{
    public class TempNewsRepository : ITempNewsRepository
    {
        private readonly NewsContext _db;
        public TempNewsRepository(NewsContext db)
        {
            this._db = db;
        }



        public async Task<OperationResult<Admin_GetTempNewsArticleDomainDTO>> GetAdminTempNewsArticle(Admin_TempNewsArticle_FilterDTO filter, CancellationToken cancellationToken = default)
        {
            OperationResult<Admin_GetTempNewsArticleDomainDTO> Op = new("GetAdminTempNewsArticle");
            if (filter != null && filter.NewsTag != null && !new List<int>() { 0, 1, 2, 3, 4, 5 }.Any(x => x == (int)filter.NewsTag))
            {
                return Op.Failed("فیلتر نوع اعلان بدرستی ارسال نشده است");
            }
            try
            {
                var totalCount = Convert.ToInt64((from news in await _db.TempNewsArticles.Include(x => x.Medias).ToListAsync(cancellationToken: cancellationToken)
                                                  where
                                                       filter == null ||
                                                       (
                                                           (string.IsNullOrEmpty(filter.Search) || string.IsNullOrWhiteSpace(filter.Search) || (news.Title.ToLower().Contains(filter.Search.ToLower()) || (string.IsNullOrEmpty(news.Description) || string.IsNullOrWhiteSpace(news.Description) ? "" : news.Description).ToLower().Contains(filter.Search.ToLower()))) &&
                                                           (filter.NewsTag == null || (int)news.NewsTag == (int)filter.NewsTag) &&
                                                           (filter.Important == null || news.Important == filter.Important) &&
                                                           (filter.IncludesDate == null || news.FromDate.ResetTime() <= filter.IncludesDate.ResetTime() && news.ToDate.ResetTime() >= filter.IncludesDate.ResetTime()) &&
                                                           (filter.CreationDate == null || news.CreationDate.ResetTime() == filter.CreationDate.ResetTime())
                                                       )
                                                  select news.Id).ToList().Count);
                if (totalCount <= 0)
                {
                    return Op.Succeed("دریافت بورد موقت ساختمان با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", System.Net.HttpStatusCode.NoContent);
                }
                var data = (from news in await _db.TempNewsArticles.Include(x => x.Medias).ToListAsync(cancellationToken: cancellationToken)
                            where
                                 filter == null ||
                                 (
                                     (string.IsNullOrEmpty(filter.Search) || string.IsNullOrWhiteSpace(filter.Search) || (news.Title.ToLower().Contains(filter.Search.ToLower()) || (string.IsNullOrEmpty(news.Description) || string.IsNullOrWhiteSpace(news.Description) ? "" : news.Description).ToLower().Contains(filter.Search.ToLower()))) &&
                                     (filter.NewsTag == null || (int)news.NewsTag == (int)filter.NewsTag) &&
                                     (filter.Important == null || news.Important == filter.Important) &&
                                     (filter.IncludesDate == null || news.FromDate.ResetTime() <= filter.IncludesDate.ResetTime() && news.ToDate.ResetTime() >= filter.IncludesDate.ResetTime()) &&
                                     (filter.CreationDate == null || news.CreationDate.ResetTime() == filter.CreationDate.ResetTime())
                                 )
                            select new GetTempNewsArticleDomainDTO
                            {
                                Active = news.Active,
                                CreationDate = news.CreationDate,
                                Creator = news.Creator,
                                Description = news.Description,
                                FromDate = news.FromDate,
                                Id = news.Id,
                                Important = news.Important,
                                Medias = news.Medias != null && news.Medias.Count > 0 ? news.Medias.Select(x => new MediaDTO
                                {
                                    MediaType = x.MediaType,
                                    Url = x.Url,
                                }).ToList() : new List<MediaDTO>(),
                                ModificationDate = news.ModificationDate,
                                Modifier = news.Modifier,
                                NewsTag = news.NewsTag,
                                Pinned = news.Pinned,
                                Priority = news.Priority,
                                Thumbnail = string.IsNullOrEmpty(news.ThumbnailUrl) || string.IsNullOrWhiteSpace(news.ThumbnailUrl) ? null : new MediaDTO
                                {
                                    MediaType = news.ThumbnailMediaType == null ? MediaType.IMAGE : (MediaType)news.ThumbnailMediaType,
                                    Url = news.ThumbnailUrl,
                                },
                                Title = news.Title,
                                ToDate = news.ToDate,
                            }
                      ).OrderBy(x => x.Priority).OrderByDescending(x => x.CreationDate).Skip((Convert.ToInt32(filter.PageNumber) - 1) * Convert.ToInt32(filter.PageSize)).Take(Convert.ToInt32(filter.PageSize)).ToList();

                return Op.Succeed("دریافت بورد موقت ساختمان با موفقیت انجام شد", new Admin_GetTempNewsArticleDomainDTO
                {
                    Data = data,
                    TotalCount = totalCount,
                });
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت بورد موقت ساختمان با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
        public async Task<OperationResult<Admin_GetSingleTempNewsArticleDomainDTO>> GetAdminSingleTempNewsArticle(long Id, CancellationToken cancellationToken = default)
        {
            OperationResult<Admin_GetSingleTempNewsArticleDomainDTO> Op = new("GetAdminSingleTempNewsArticle");
            if (Id <= 0)
            {
                return Op.Failed("شناسه اعلان موقت بدرستی ارسال نشده است");
            }
            try
            {

                var news = await _db.TempNewsArticles.Include(x => x.Medias).FirstOrDefaultAsync(x => x.Id == Id, cancellationToken: cancellationToken);
                if (news == null)
                {
                    return Op.Failed("شناسه اعلان موقت بدرستی ارسال نشده است", System.Net.HttpStatusCode.NotFound);
                }
                return Op.Succeed("دریافت اعلان موقت ساختمان با موفقیت انجام شد", new Admin_GetSingleTempNewsArticleDomainDTO
                {
                    Active = news.Active,
                    CreationDate = news.CreationDate,
                    Creator = news.Creator,
                    Description = news.Description,
                    FromDate = news.FromDate,
                    Id = news.Id,
                    Important = news.Important,
                    Medias = news.Medias != null && news.Medias.Count > 0 ? news.Medias.Select(x => new MediaDTO
                    {
                        MediaType = x.MediaType,
                        Url = x.Url,
                    }).ToList() : new List<MediaDTO>(),
                    ModificationDate = news.ModificationDate,
                    Modifier = news.Modifier,
                    NewsTag = news.NewsTag,
                    Pinned = news.Pinned,
                    Priority = news.Priority,
                    Thumbnail = string.IsNullOrEmpty(news.ThumbnailUrl) || string.IsNullOrWhiteSpace(news.ThumbnailUrl) ? null : new MediaDTO
                    {
                        MediaType = news.ThumbnailMediaType == null ? MediaType.IMAGE : (MediaType)news.ThumbnailMediaType,
                        Url = news.ThumbnailUrl,
                    },
                    Title = news.Title,
                    ToDate = news.ToDate,
                    NewsTitle = this._GetTagData((int)news.NewsTag).Title,
                    NewsTagDisplayTitle = this._GetTagData((int)news.NewsTag).DisplayTitle
                });
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اعلان موقت ساختمان با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
        public async Task<OperationResult<Customer_GetTempNewsArticleDomainDTO>> GetCustomerTempNewsArticle(Customer_TempNewsArticle_FilterDTO filter, CancellationToken cancellationToken = default)
        {
            OperationResult<Customer_GetTempNewsArticleDomainDTO> Op = new("GetCustomerTempNewsArticle");
            try
            {
                var now = DateTime.Now;
                List<Customer_GetTempNewsArticleDomainDTO> data = new();
                List<int> tags = filter != null && filter.NewsTags != null && filter.NewsTags.Any() ? filter.NewsTags.Select(x => (int)x).Distinct().ToList() : new List<int> { 0, 1, 2, 3, 4, 5 };

                if (filter != null && filter.PageSize != null && filter.PageSize > 0)
                {
                    if (filter.PageNumber == null || filter.PageNumber <= 0)
                    {
                        filter.PageNumber = 1;
                    }
                    data.AddRange((from news in await _db.TempNewsArticles.Include(x => x.Medias).ToListAsync(cancellationToken: cancellationToken)
                                   join tag in tags
                                   on (int)news.NewsTag equals tag
                                   where
                                        news.Active && news.FromDate.ResetTime() <= now.ResetTime() && news.ToDate.ResetTime() >= now.ResetTime() && (filter == null ||
                                        (
                                            (string.IsNullOrEmpty(filter.Search) || string.IsNullOrWhiteSpace(filter.Search) || (news.Title.ToLower().Contains(filter.Search.ToLower()) || (string.IsNullOrEmpty(news.Description) || string.IsNullOrWhiteSpace(news.Description) ? "" : news.Description).ToLower().Contains(filter.Search.ToLower()))) &&
                                            (filter.Important == null || news.Important == filter.Important) &&
                                            (filter.CreationDate == null || news.CreationDate.ResetTime() == filter.CreationDate.ResetTime())
                                        ))
                                   select new Customer_GetTempNewsArticleDomainDTO
                                   {
                                       CreationDate = news.CreationDate,
                                       Description = news.Description,
                                       FromDate = news.FromDate,
                                       Id = news.Id,
                                       Important = news.Important,
                                       Medias = news.Medias != null && news.Medias.Count > 0 ? news.Medias.Select(x => new MediaDTO
                                       {
                                           MediaType = x.MediaType,
                                           Url = x.Url,
                                       }).ToList() : new List<MediaDTO>(),
                                       ModificationDate = news.ModificationDate,
                                       NewsTag = news.NewsTag,
                                       Pinned = news.Pinned,
                                       Priority = news.Priority,
                                       Thumbnail = string.IsNullOrEmpty(news.ThumbnailUrl) || string.IsNullOrWhiteSpace(news.ThumbnailUrl) ? null : new MediaDTO
                                       {
                                           MediaType = news.ThumbnailMediaType == null ? MediaType.IMAGE : (MediaType)news.ThumbnailMediaType,
                                           Url = news.ThumbnailUrl,
                                       },
                                       Title = news.Title,
                                       ToDate = news.ToDate,
                                   }
                    ).OrderBy(x => x.Priority).OrderByDescending(x => x.CreationDate).Skip((Convert.ToInt32(filter.PageNumber) - 1) * Convert.ToInt32(filter.PageSize)).Take(Convert.ToInt32(filter.PageSize)).ToList());
                }
                else
                {
                    data.AddRange((from news in await _db.TempNewsArticles.Include(x => x.Medias).ToListAsync(cancellationToken: cancellationToken)
                                   join tag in tags
                                   on (int)news.NewsTag equals tag
                                   where
                                        news.Active && news.FromDate.ResetTime() <= now.ResetTime() && news.ToDate.ResetTime() >= now.ResetTime() && (filter == null ||
                                        (
                                            (string.IsNullOrEmpty(filter.Search) || string.IsNullOrWhiteSpace(filter.Search) || (news.Title.ToLower().Contains(filter.Search.ToLower()) || (string.IsNullOrEmpty(news.Description) || string.IsNullOrWhiteSpace(news.Description) ? "" : news.Description).ToLower().Contains(filter.Search.ToLower()))) &&
                                            (filter.Important == null || news.Important == filter.Important) &&
                                            (filter.CreationDate == null || news.CreationDate.ResetTime() == filter.CreationDate.ResetTime())
                                        ))
                                   select new Customer_GetTempNewsArticleDomainDTO
                                   {
                                       CreationDate = news.CreationDate,
                                       Description = news.Description,
                                       FromDate = news.FromDate,
                                       Id = news.Id,
                                       Important = news.Important,
                                       Medias = news.Medias != null && news.Medias.Count > 0 ? news.Medias.Select(x => new MediaDTO
                                       {
                                           MediaType = x.MediaType,
                                           Url = x.Url,
                                       }).ToList() : new List<MediaDTO>(),
                                       ModificationDate = news.ModificationDate,
                                       NewsTag = news.NewsTag,
                                       Pinned = news.Pinned,
                                       Priority = news.Priority,
                                       Thumbnail = string.IsNullOrEmpty(news.ThumbnailUrl) || string.IsNullOrWhiteSpace(news.ThumbnailUrl) ? null : new MediaDTO
                                       {
                                           MediaType = news.ThumbnailMediaType == null ? MediaType.IMAGE : (MediaType)news.ThumbnailMediaType,
                                           Url = news.ThumbnailUrl,
                                       },
                                       Title = news.Title,
                                       ToDate = news.ToDate,
                                   }
                   ).OrderBy(x => x.Priority).OrderByDescending(x => x.CreationDate).ToList());
                }
                if (data == null || data.Count == 0)
                {
                    return Op.Succeed("دریافت بورد موقت ساختمان با موفقیت انجام شد، اطلاعاتی جهت نمایش وجود ندارد", System.Net.HttpStatusCode.NoContent);
                }
                return Op.Succeed("دریافت بورد موقت ساختمان با موفقیت انجام شد", data);
            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت بورد موقت ساختمان با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
        public async Task<OperationResult<Customer_GetSingleTempNewsArticleDomainDTO>> GetSingleTempNewsArticle(long Id, CancellationToken cancellationToken = default)
        {
            OperationResult<Customer_GetSingleTempNewsArticleDomainDTO> Op = new("GetSingleTempNewsArticle");
            if (Id <= 0)
            {
                return Op.Failed("شناسه اعلان موقت بدرستی ارسال نشده است");
            }
            try
            {
                var now = DateTime.Now;
                var news = await _db.TempNewsArticles.Include(x => x.Medias).FirstOrDefaultAsync(x => x.Id == Id, cancellationToken: cancellationToken);
                if (news == null)
                {
                    return Op.Failed("شناسه اعلان موقت بدرستی ارسال نشده است", System.Net.HttpStatusCode.NotFound);
                }
                
                //if (!news.Active || news.FromDate.ResetTime() <= now.ResetTime() && news.ToDate.ResetTime() >= now.ResetTime())
                //{
                //    return Op.Failed("شناسه اعلان موقت بدرستی ارسال نشده است", System.Net.HttpStatusCode.NotFound);
                //}

                return Op.Succeed("دریافت اعلان موقت ساختمان با موفقیت انجام شد", new Customer_GetSingleTempNewsArticleDomainDTO
                {
                    CreationDate = news.CreationDate,
                    Description = news.Description,
                    FromDate = news.FromDate,
                    Id = news.Id,
                    Important = news.Important,
                    Medias = news.Medias != null && news.Medias.Count > 0 ? news.Medias.Select(x => new MediaDTO
                    {
                        MediaType = x.MediaType,
                        Url = x.Url,
                    }).ToList() : new List<MediaDTO>(),
                    ModificationDate = news.ModificationDate,
                    NewsTag = news.NewsTag,
                    Pinned = news.Pinned,
                    Priority = news.Priority,
                    Thumbnail = string.IsNullOrEmpty(news.ThumbnailUrl) || string.IsNullOrWhiteSpace(news.ThumbnailUrl) ? null : new MediaDTO
                    {
                        MediaType = news.ThumbnailMediaType == null ? MediaType.IMAGE : (MediaType)news.ThumbnailMediaType,
                        Url = news.ThumbnailUrl,
                    },
                    Title = news.Title,
                    ToDate = news.ToDate,
                    NewsTitle = this._GetTagData((int)news.NewsTag).Title,
                    NewsTagDisplayTitle = this._GetTagData((int)news.NewsTag).DisplayTitle
                });

            }
            catch (Exception ex)
            {
                return Op.Failed("دریافت اعلان موقت ساختمان با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
        public async Task<OperationResult<object>> PostTempNewsArticle(PostTempNewsArticleDomainDTO model, int adminId, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("PostTempNewsArticle");
            if (adminId <= 0)
            {
                return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی جهت ثبت خبر موقت ارسال نشده است");
            }
            if (string.IsNullOrEmpty(model.Title) || string.IsNullOrWhiteSpace(model.Title))
            {
                return Op.Failed("ارسال عنوان خبر اجباری است");
            }
            if (model.Title.Length < 2 || model.Title.Length > 270)
            {
                return Op.Failed("تعداد کاراکتر مجاز عنوان خبر موقت بین 2 تا 270 کاراکتر است");
            }
            if (!string.IsNullOrEmpty(model.Description) && !string.IsNullOrWhiteSpace(model.Description) && (model.Description.Length < 5 || model.Description.Length > 2000))
            {
                return Op.Failed("تعداد کاراکتر مجاز توضیحات خبر موقت بین 5 تا 2000 کاراکتر است");
            }
            if (!new List<int>() { 0, 1, 2, 3, 4, 5 }.Any(x => x == (int)model.NewsTag))
            {
                return Op.Failed("شناسه نوع خبر موقت بدرستی ارسال نشده است");
            }
            if (model.FromDate.ResetTime() > model.ToDate.ResetTime())
            {
                return Op.Failed("تاریخ شروع و پایان خبر موقت بدرستی ارسال نشده است");
            }
            if (model.Medias == null || !model.Medias.Any())
            {
                return Op.Failed("ارسال حداقل یک فایل برای خبر اجباری است");
            }
            if (model.Medias.Any(x => string.IsNullOrEmpty(x.Url) || string.IsNullOrWhiteSpace(x.Url)))
            {
                return Op.Failed($"آدرس فایل خبر بدرستی ارسال نشده است، خطا در ردیف: {model.Medias.FindIndex(x => string.IsNullOrEmpty(x.Url) || string.IsNullOrWhiteSpace(x.Url)) + 1}");
            }
            if (model.Medias.Any(y => !new List<int>() { (int)MediaType.IMAGE, (int)MediaType.GIF, (int)MediaType.VIDEO }.Any(x => x == (int)y.MediaType)))
            {
                return Op.Failed($"نوع فایل خبر بدرستی ارسال نشده است، خطا در ردیف: {model.Medias.FindIndex(y => !new List<int>() { (int)MediaType.IMAGE, (int)MediaType.GIF, (int)MediaType.VIDEO }.Any(x => x == (int)y.MediaType)) + 1}");
            }
            if (model.Thumbnail != null)
            {
                if (string.IsNullOrEmpty(model.Thumbnail.Url) || string.IsNullOrWhiteSpace(model.Thumbnail.Url))
                {
                    return Op.Failed("آدرس فایل پیش نمایش خبر بدرستی ارسال نشده است");
                }
                if (!new List<int>() { (int)MediaType.IMAGE, (int)MediaType.GIF, (int)MediaType.VIDEO }.Any(x => x == (int)model.Thumbnail.MediaType))
                {
                    return Op.Failed("نوع فایل پیش نمایش خبر بدرستی ارسال نشده است");
                }
            }
            else
            {
                model.Thumbnail = model.Medias.FirstOrDefault();
            }
            try
            {
                await this._db.TempNewsArticles.AddAsync(new TempNewsArticle
                {
                    Active = model.Active == null || Convert.ToBoolean(model.Active),
                    CreationDate = Op.OperationDate,
                    Creator = adminId,
                    Description = string.IsNullOrEmpty(model.Description) || string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
                    FromDate = model.FromDate,
                    Important = model.Important != null && model.Important != false,
                    ModificationDate = null,
                    Modifier = null,
                    NewsTag = model.NewsTag,
                    Pinned = model.Pinned != null && model.Pinned != false,
                    Priority = model.Priority != null && model.Priority > 0 ? Convert.ToInt64(model.Priority) : 1,
                    ThumbnailMediaType = model.Thumbnail.MediaType,
                    ThumbnailUrl = model.Thumbnail.Url,
                    Title = model.Title.Trim(),
                    ToDate = model.ToDate,
                }, cancellationToken);
                await this._db.SaveChangesAsync(cancellationToken);
                var addedNews = await _db.TempNewsArticles.Where(x => x.CreationDate == Op.OperationDate).FirstOrDefaultAsync(cancellationToken: cancellationToken);
                if (addedNews == null)
                {
                    return Op.Failed("ثبت خبر موقت با مشکل مواجه شده است", System.Net.HttpStatusCode.InternalServerError);
                }
                await this._db.Medias.AddRangeAsync((from media in model.Medias
                                                     select new Media
                                                     {
                                                         MediaType = media.MediaType,
                                                         TempNewsArticleId = addedNews.Id,
                                                         Url = media.Url
                                                     }).ToList(), cancellationToken);
                await this._db.SaveChangesAsync(cancellationToken);
                return Op.Succeed("ثبت خبر موقت با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("ثبت خبر موقت با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
        public async Task<OperationResult<object>> UpdateTempNewsArticle(UpdateTempNewsArticleDomainDTO model, int adminId, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("UpdateTempNewsArticle");
            if (adminId <= 0)
            {
                return Op.Failed("شناسه ادمین بدرستی ارسال نشده است");
            }
            if (model == null)
            {
                return Op.Failed("اطلاعاتی جهت بروزرسانی خبر موقت ارسال نشده است");
            }
            if (model.Id <= 0)
            {
                return Op.Failed("شناسه خبر موقت بدرستی ارسال نشده است");
            }
            if ((string.IsNullOrEmpty(model.Title) || string.IsNullOrWhiteSpace(model.Title)) &&
                (string.IsNullOrEmpty(model.Description) || string.IsNullOrWhiteSpace(model.Description)) &&
                (model.Important == null) &&
                (model.Pinned == null) &&
                (model.Active == null) &&
                (model.Priority == null) &&
                (model.FromDate == null) &&
                (model.ToDate == null) &&
                (model.Thumbnail == null) &&
                (model.Medias == null))
            {
                return Op.Failed("اطلاعاتی جهت بروزرسانی ارسال نشده است");
            }
            if (!string.IsNullOrEmpty(model.Title) && !string.IsNullOrWhiteSpace(model.Title) && (model.Title.Length < 2 || model.Title.Length > 270))
            {
                return Op.Failed("تعداد کاراکتر مجاز عنوان خبر موقت بین 2 تا 270 کاراکتر است");
            }
            if (!string.IsNullOrEmpty(model.Description) && !string.IsNullOrWhiteSpace(model.Description) && (model.Description.Length < 5 || model.Description.Length > 2000))
            {
                return Op.Failed("تعداد کاراکتر مجاز توضیحات خبر موقت بین 5 تا 2000 کاراکتر است");
            }
            if (model.NewsTag != null && !new List<int>() { 0, 1, 2, 3, 4, 5 }.Any(x => x == (int)model.NewsTag))
            {
                return Op.Failed("شناسه نوع خبر موقت بدرستی ارسال نشده است");
            }
            if (model.FromDate != null && model.ToDate != null && model.FromDate.ResetTime() > model.ToDate.ResetTime())
            {
                return Op.Failed("تاریخ شروع و پایان خبر موقت بدرستی ارسال نشده است");
            }
            if (model.Medias != null && model.Medias.Any())
            {
                if (model.Medias.Any(x => string.IsNullOrEmpty(x.Url) || string.IsNullOrWhiteSpace(x.Url)))
                {
                    return Op.Failed($"آدرس فایل خبر بدرستی ارسال نشده است، خطا در ردیف: {model.Medias.FindIndex(x => string.IsNullOrEmpty(x.Url) || string.IsNullOrWhiteSpace(x.Url)) + 1}");
                }
                if (model.Medias.Any(y => !new List<int>() { (int)MediaType.IMAGE, (int)MediaType.GIF, (int)MediaType.VIDEO }.Any(x => x == (int)y.MediaType)))
                {
                    return Op.Failed($"نوع فایل خبر بدرستی ارسال نشده است، خطا در ردیف: {model.Medias.FindIndex(y => !new List<int>() { (int)MediaType.IMAGE, (int)MediaType.GIF, (int)MediaType.VIDEO }.Any(x => x == (int)y.MediaType)) + 1}");
                }
            }
            if (model.Thumbnail != null)
            {
                if (string.IsNullOrEmpty(model.Thumbnail.Url) || string.IsNullOrWhiteSpace(model.Thumbnail.Url))
                {
                    return Op.Failed("آدرس فایل پیش نمایش خبر بدرستی ارسال نشده است");
                }
                if (!new List<int>() { (int)MediaType.IMAGE, (int)MediaType.GIF, (int)MediaType.VIDEO }.Any(x => x == (int)model.Thumbnail.MediaType))
                {
                    return Op.Failed("نوع فایل پیش نمایش خبر بدرستی ارسال نشده است");
                }
            }
            try
            {
                var news = await this._db.TempNewsArticles.Include(x => x.Medias).FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken: cancellationToken);
                if (news == null)
                {
                    return Op.Failed("شناسه خبر موقت بدرستی ارسال نشده است", System.Net.HttpStatusCode.NotFound);
                }
                if ((model.FromDate != null ? Convert.ToDateTime(model.FromDate) : news.FromDate).ResetTime() > (model.ToDate != null ? Convert.ToDateTime(model.ToDate) : news.ToDate).ResetTime())
                {
                    return Op.Failed("تاریخ شروع و پایان خبر موقت بدرستی ارسال نشده است");
                }
                if (!string.IsNullOrEmpty(model.Title) && !string.IsNullOrWhiteSpace(model.Title))
                {
                    news.Title = model.Title.Trim();
                }
                if (!string.IsNullOrEmpty(model.Description) && !string.IsNullOrWhiteSpace(model.Description))
                {
                    news.Description = model.Description.Trim();
                }
                if (model.NewsTag != null)
                {
                    news.NewsTag = (NewsTagType)model.NewsTag;
                }
                if (model.Important != null)
                {
                    news.Important = Convert.ToBoolean(model.Important);
                }
                if (model.Pinned != null)
                {
                    news.Pinned = Convert.ToBoolean(model.Pinned);
                }
                if (model.Active != null)
                {
                    news.Active = Convert.ToBoolean(model.Active);
                }
                if (model.Priority != null)
                {
                    news.Priority = Convert.ToInt32(model.Priority);
                }
                if (model.FromDate != null)
                {
                    news.FromDate = Convert.ToDateTime(model.FromDate);
                }
                if (model.ToDate != null)
                {
                    news.ToDate = Convert.ToDateTime(model.ToDate);
                }
                if (model.Thumbnail != null)
                {
                    news.ThumbnailUrl = model.Thumbnail.Url;
                    news.ThumbnailMediaType = model.Thumbnail.MediaType;
                }
                news.ModificationDate = Op.OperationDate;
                news.Modifier = adminId;
                this._db.Entry<TempNewsArticle>(news).State = EntityState.Modified;

                if (model.Medias != null && model.Medias.Any())
                {
                    this._db.Medias.RemoveRange(await _db.Medias.Where(x => x.TempNewsArticleId == news.Id).ToListAsync(cancellationToken: cancellationToken));
                    await _db.Medias.AddRangeAsync((from media in model.Medias
                                                    select new Media
                                                    {
                                                        MediaType = media.MediaType,
                                                        TempNewsArticleId = news.Id,
                                                        Url = media.Url,
                                                    }).ToList(), cancellationToken);
                }
                await _db.SaveChangesAsync(cancellationToken);
                return Op.Succeed("بروزرسانی خبر موقت با موفقیت انجام شد");

            }
            catch (Exception ex)
            {
                return Op.Failed("بروزرسانی خبر موقت با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }

        }
        public async Task<OperationResult<object>> DeleteTempNewsArticle(DeleteTempNewsArticleDomainDTO model, CancellationToken cancellationToken = default)
        {
            OperationResult<object> Op = new("DeleteTempNewsArticle");
            if (model == null || model.NewsId <= 0)
            {
                return Op.Failed("شناسه خبر موقت بدرستی ارسال نشده است");
            }
            try
            {
                var news = await _db.TempNewsArticles.FirstOrDefaultAsync(x => x.Id == model.NewsId, cancellationToken: cancellationToken);
                if (news == null)
                {
                    return Op.Failed("شناسه خبر موقت بدرستی ارسال نشده است", System.Net.HttpStatusCode.NotFound);
                }
                this._db.Medias.RemoveRange(await this._db.Medias.Where(x => x.TempNewsArticleId == model.NewsId).ToListAsync(cancellationToken: cancellationToken));
                this._db.TempNewsArticles.Remove(news);
                await _db.SaveChangesAsync(cancellationToken);
                return Op.Succeed("حذف خبر موقت با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                return Op.Failed("حذف خبر موقت با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
        private (string Title, string DisplayTitle) _GetTagData(int id)
        {
            return id switch
            {
                3 => (Title: NewsTagType.MAINTENANCE.ToString(), DisplayTitle: "فنی و مهندسی"),
                4 => (Title: NewsTagType.CONCIERGE.ToString(), DisplayTitle: "کانسیرژ"),
                2 => (Title: NewsTagType.MANAGMENT.ToString(), DisplayTitle: "مدیر ساختمان"),
                1 => (Title: NewsTagType.EVENT.ToString(), DisplayTitle: "ایونت"),
                5 => (Title: NewsTagType.PERFORMANCE_REPORTS.ToString(), DisplayTitle: "گزارش عملکرد"),
                0 => (Title: NewsTagType.ETC.ToString(), DisplayTitle: "سایر"),
                _ => (Title: "", DisplayTitle: ""),
            };
        }

       
    }
}
