using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Entity.Utils;
using Liberty.GmoPaymentGateway.Models.Requests;
using Liberty.GmoPaymentGateway.Services;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Contexts;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;
using SiteEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Site;

namespace Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;

public class MockBookingData(
    AppWebApplicationFactory<TestStartup> factory,
    HttpClient client,
    Facility facilityInfo
)
{
    private async Task<IEnumerable<SiteEntity>> CreateDestinationsAsync()
    {
        var destinationRepo = factory.GetRequiredService<IFacilitySiteRepository>()
            ?? throw new ArgumentException(nameof(IFacilitySiteRepository));

        var destinationsToCreate = new List<FacilitySite>();
        for (var i = 1; i <= 5; i++)
        {
            destinationsToCreate.Add(
                new FacilitySite
                {
                    FacilityId = facilityInfo.Id,
                    Site = new SiteEntity
                    {
                        Name = new MultilingualText { { TestUtil.DefaultLanguageCode, $"Site {i}" } },
                        Description = $"Site description {i}",
                        IsEnabled = true
                    },
                    IsEnabled = true
                }
            );
        }

        var createdDestinations = await destinationRepo.AddRangeAsync(
            destinationsToCreate,
            true
        );

        return createdDestinations.Select(x => x.Site!);
    }

    private async Task<IEnumerable<OptionItem>> CreateOptionItemsAsync()
    {
        var optionItemOfFacilityRepo = factory.GetRequiredService<IFacilityOptionItemRepository>()
            ?? throw new ArgumentException(nameof(IOptionItemRepository));

        var optionItemsToCreate = new List<FacilityOptionItem>();
        for (var i = 1; i <= 5; i++)
        {
            optionItemsToCreate.Add(
                new FacilityOptionItem
                {
                    FacilityId = facilityInfo.Id,
                    OptionItem = new OptionItem
                    {
                        Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Option item ${i}" } },
                        Description = $"Option description {i}",
                        BaseNumber = 10 + i,
                        Price = 100 + i,
                        IsEnabled = true
                    },
                    IsEnabled = true
                }
            );
        }

        var createdOptionItems = await optionItemOfFacilityRepo.AddRangeAsync(
            optionItemsToCreate,
            true
        );

        return createdOptionItems.Select(x => x.OptionItem!);
    }

    private async Task<IEnumerable<RoomGroup>> CreateRoomGroupsAsync(
        IEnumerable<SiteEntity> destinations
    )
    {
        var bedTypeRepo = factory.GetRequiredService<IBedTypeRepository>()
            ?? throw new ArgumentException(nameof(IBedTypeRepository));
        var roomGroupOfFacilityRepo = factory.GetRequiredService<IFacilityRoomGroupRepository>()
            ?? throw new ArgumentException(nameof(IFacilityRoomGroupRepository));

        var bedTypes = await bedTypeRepo.GetAllAsync();
        var destinationsList = destinations.ToList();

        var roomGroupsToCreate = new List<FacilityRoomGroup>();
        for (var i = 1; i <= 5; i++)
        {
            roomGroupsToCreate.Add(
                new FacilityRoomGroup
                {
                    FacilityId = facilityInfo.Id,
                    RoomGroup = new RoomGroup
                    {
                        Name = new MultilingualText { { TestUtil.DefaultLanguageCode, $"Room group {i}" } },
                        Description = new MultilingualText { { TestUtil.DefaultLanguageCode, $"Room description {i}" } },
                        CapacityMin = 1,
                        CapacityMax = 10 + i,
                        BaseNumber = 10 + i,
                        Size = 15 + i,
                        RoomGroupSizeUnitType = RoomGroupSizeUnitTypes.M2,
                        RoomGroupBedTypes =
                        [
                            .. bedTypes.Select(
                                x => new RoomGroupBedType
                                {
                                    BedTypeId = x.Id,
                                    Number = 50 + i,
                                    IsEnabled = true
                                }
                            )
                        ],
                        RoomGroupSites =
                        [
                            .. destinationsList.Select(
                                x => new RoomGroupSite
                                {
                                    SiteId = x.Id,
                                    IsEnabled = true
                                }
                            )
                        ],
                        Tag = string.Join(
                            ",",
                            $"Tag 1{i}",
                            $"Tag 2{i}",
                            $"Tag 3{i}"
                        ),
                        IsEnabledSmoking = true,
                        IsEnabled = true
                    },
                    IsEnabled = true
                }
            );
        }

        var createdRoomGroups = await roomGroupOfFacilityRepo.AddRangeAsync(
            roomGroupsToCreate,
            true
        );

        return createdRoomGroups.Select(x => x.RoomGroup!);
    }

    private async Task<IEnumerable<Cancellation>> CreateCancellationsAsync()
    {
        var cancellationRepo = factory.GetRequiredService<IFacilityCancellationRepository>()
            ?? throw new ArgumentException(nameof(IFacilityCancellationRepository));

        var cancellationsToCreate = new List<FacilityCancellation>();
        for (var i = 1; i <= 5; i++)
        {
            cancellationsToCreate.Add(
                new FacilityCancellation
                {
                    FacilityId = facilityInfo.Id,
                    Cancellation = new Cancellation
                    {
                        Name = new MultilingualText { { TestUtil.DefaultLanguageCode, $"Cancellation {i}" } },
                        Description = new MultilingualText { { TestUtil.DefaultLanguageCode, $"Cancellation description {i}" } },
                        IsEnabled = true
                    },
                    IsEnabled = true
                }
            );
        }

        var createdCancellations = await cancellationRepo.AddRangeAsync(
            cancellationsToCreate,
            true
        );

        return createdCancellations.Select(x => x.Cancellation!);
    }

    private async Task<IEnumerable<Question>> CreateQuestionsAsync()
    {
        var questionRepo = factory.GetRequiredService<IFacilityQuestionRepository>()
            ?? throw new ArgumentException(nameof(IFacilityQuestionRepository));

        var questionsToCreate = new List<FacilityQuestion>();
        for (var i = 1; i <= 5; i++)
        {
            questionsToCreate.Add(
                new FacilityQuestion
                {
                    FacilityId = facilityInfo.Id,
                    Question = new Question
                    {
                        Name = new MultilingualText { { TestUtil.DefaultLanguageCode, $"Question {i}" } },
                        Description = new MultilingualText { { TestUtil.DefaultLanguageCode, $"Question description {i}" } },
                        QuestionType = QuestionTypes.Unknown,
                        IsEnabled = true
                    },
                    IsEnabled = true
                }
            );
        }

        var createdQuestions = await questionRepo.AddRangeAsync(
            questionsToCreate,
            true
        );

        return createdQuestions.Select(x => x.Question!);
    }

    private async Task<IEnumerable<Category>> CreatePlanCategoriesAsync()
    {
        var categoryRepo = factory.GetRequiredService<IFacilityCategoryRepository>()
            ?? throw new ArgumentException(nameof(IFacilityCategoryRepository));

        var categoriesToCreate = new List<FacilityCategory>();
        for (var i = 1; i <= 5; i++)
        {
            categoriesToCreate.Add(
                new FacilityCategory
                {
                    FacilityId = facilityInfo.Id,
                    Category = new Category
                    {
                        CategoryType = CategoryTypes.Plan,
                        Name = new MultilingualText { { TestUtil.DefaultLanguageCode, $"Plan category name {i}" } },
                        Description = new MultilingualText { { TestUtil.DefaultLanguageCode, $"Plan category description {i}" } },
                        IsEnabled = true
                    },
                    IsEnabled = true
                }
            );
        }

        var createdPlanCategories = await categoryRepo.AddRangeAsync(
            categoriesToCreate,
            true
        );

        return createdPlanCategories.Select(x => x.Category!);
    }

    private async Task<IEnumerable<Plan>> CreatePlansAsync()
    {
        var planRepo = factory.GetRequiredService<IFacilityPlanRepository>()
            ?? throw new ArgumentException(nameof(IFacilityPlanRepository));

        var destinationsList = (await CreateDestinationsAsync()).ToList();
        var planCategoriesList = (await CreatePlanCategoriesAsync()).ToList();
        var roomGroupsList = (await CreateRoomGroupsAsync(destinationsList)).ToList();
        var optionItemsList = (await CreateOptionItemsAsync()).ToList();
        var questionsList = (await CreateQuestionsAsync()).ToList();
        var cancellation = (await CreateCancellationsAsync()).First();
        await CreateCancellationCancellationDataAsync(cancellation.Id);

        var plansToCreate = new List<FacilityPlan>();
        for (var i = 1; i <= 5; i++)
        {
            var plan = new Plan
            {
                // Update basic setting
                Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), $"Plan name {i}" } },
                Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), $"Plan description {i}" } },
                IsEnabled = true,
                // Update display
                PlanCategories =
                [
                    .. planCategoriesList.Select(
                        x => new PlanCategory
                        {
                            CategoryId = x.Id,
                            IsEnabled = true
                        }
                    )
                ],
                Tag = new MultilingualText
                {
                    { LanguageHeaderUtil.GetLanguageCodeFromHeader(), string.Join(",", $"Tag 1{i}", $"Tag 2{i}", $"Tag 3{i}") }
                },
                // Update room types
                PlanRoomGroups =
                [
                    .. roomGroupsList.Select(
                        x => new PlanRoomGroup
                        {
                            RoomGroupId = x.Id,
                            IsEnabled = true
                        }
                    )
                ],
                // Update publish accept
                UseDisplayDate = false,
                DisplayDateStart = AppDate.GetId(DateTime.Now),
                DisplayDateEnd = AppDate.GetId(DateTime.Now.AddDays(15)),
                UseAcceptDate = false,
                AcceptDateStart = AppDate.GetId(DateTime.Now),
                AcceptDateEnd = AppDate.GetId(DateTime.Now.AddDays(15)),
                AcceptDays = 100,
                AcceptMonths = 12,
                AcceptEndLimitType = PlanAcceptEndLimitTypes.AfterDays,
                ReceptionDayLimit = null,
                ReceptionLimit = null,
                PlanSites =
                [
                    .. destinationsList.Select(
                        x => new PlanSite
                        {
                            SiteId = x.Id,
                            IsEnabled = true
                        }
                    )
                ],
                // Update sale
                CheckInStart = new TimeSpan(12, 0, 0),
                CheckInEnd = new TimeSpan(18, 0, 0),
                CheckOut = new TimeSpan(12, 0, 0),
                RoomNumberDaySaleLimit = 1,
                GroupNumberDaySaleLimit = 1,
                PlanDaySaleLimitType = PlanDaySaleLimitTypes.Pair,
                UseAcceptPersonNumber = true,
                AcceptPersonNumberMin = 1,
                AcceptPersonNumberMax = 10,
                NumberOfStayLimitMin = 1,
                NumberOfStayLimitMax = 10,
                // Update payment method
                IsOnSidePayment = true,
                IsOnLinePayment = true,
                // Update option
                UseFixedOptionItem = true,
                PlanOptionItems =
                [
                    .. optionItemsList.Select(
                        x => new PlanOptionItem
                        {
                            OptionItemId = x.Id,
                            IsEnabled = true
                        }
                    )
                ],
                // Update cancellation
                IsCancelSameAccept = true,
                CancelDayLimit = 5,
                CancelLimit = new TimeSpan(12, 0, 0),
                CancellationId = cancellation.Id,
                //  Update question
                PlanQuestions =
                [
                    .. questionsList.Select(
                        x => new PlanQuestion
                        {
                            QuestionId = x.Id,
                            IsEnabled = true
                        }
                    )
                ],
                // Update special
                IsSecret = true,
                SecretWord = "Secret word"
            };

            plansToCreate.Add(
                new FacilityPlan
                {
                    FacilityId = facilityInfo.Id,
                    Plan = plan,
                    IsEnabled = true
                }
            );
        }

        var createdPlans = await planRepo.AddRangeAsync(
            plansToCreate,
            true
        );

        return createdPlans.Select(x => x.Plan!);
    }

    private async Task<IEnumerable<AppDateType>> CreateAppDateTypesAsync()
    {
        var appDateTypeRepo = factory.GetRequiredService<IFacilityAppDateTypeRepository>()
            ?? throw new ArgumentException(nameof(IFacilityAppDateTypeRepository));

        var appDateTypesToCreate = new List<FacilityAppDateType>();
        for (var i = 1; i <= 5; i++)
        {
            appDateTypesToCreate.Add(
                new FacilityAppDateType
                {
                    FacilityId = facilityInfo.Id,
                    AppDateType = new AppDateType
                    {
                        Name = $"App date type {i}",
                        ShortName = $"App date type {i}",
                        Description = $"App date type description {i}",
                        IsEnabled = true
                    },
                    IsEnabled = true
                }
            );
        }

        var createdAppDateTypes = await appDateTypeRepo.AddRangeAsync(
            appDateTypesToCreate,
            true
        );

        return createdAppDateTypes.Select(x => x.AppDateType!);
    }

    private async Task CreateSiteInRoomOfPlanAsync(
        Plan plan
    )
    {
        var planId = plan.Id;
        var roomTypeId = plan.PlanRoomGroups!.First().RoomGroupId;
        var siteId = plan.PlanSites!.First().SiteId;

        await client.PostAsync(
            $"/api/plan-prices/{planId}/room-groups/{roomTypeId}/destinations/{siteId}",
            null
        );
    }

    private async Task UpdateStandardPriceOfPlanAsync(
        Plan plan
    )
    {
        var planId = plan.Id;
        var roomTypeId = plan.PlanRoomGroups!.First().RoomGroupId;
        var siteId = plan.PlanSites!.First().SiteId;

        var appDateTypesList = (await CreateAppDateTypesAsync()).ToList();

        List<RomTypeUpdateStandardRequest> reqStandardPrices = [];
        appDateTypesList.ForEach(
            appDateType =>
            {
                reqStandardPrices.Add(
                    new RomTypeUpdateStandardRequest(
                        appDateType.Id,
                        1,
                        (int)appDateType.Id + 3,
                        500
                    )
                );

                reqStandardPrices.Add(
                    new RomTypeUpdateStandardRequest(
                        appDateType.Id,
                        1,
                        (int)appDateType.Id + 9,
                        400
                    )
                );

                reqStandardPrices.Add(
                    new RomTypeUpdateStandardRequest(
                        appDateType.Id,
                        3,
                        (int)appDateType.Id + 3,
                        450
                    )
                );
            }
        );

        await client.PatchAsync(
            $"/api/plan-prices/{planId}/room-groups/{roomTypeId}/destinations/{siteId}/standard-price",
            TestUtil.ToJsonContent(
                new RoomGroupPriceUpdateStandardPriceRequest(
                    reqStandardPrices
                )
            )
        );
    }

    private async Task UpdateChildrenPriceOfPlanAsync(
        Plan plan,
        List<PersonAgeType> personAgeTypes
    )
    {
        var planId = plan.Id;
        var roomTypeId = plan.PlanRoomGroups!.First().RoomGroupId;
        var siteId = plan.PlanSites!.First().SiteId;

        List<RoomTypeUpdateChildrenPersonAgeTypeRequest> reqChildrenPrices = [];
        personAgeTypes.ForEach(
            personAgeType =>
            {
                reqChildrenPrices.Add(
                    new RoomTypeUpdateChildrenPersonAgeTypeRequest(
                        personAgeType.Id,
                        true,
                        true,
                        PriceSettingTypes.Discount,
                        200
                    )
                );
            }
        );

        await client.PatchAsync(
            $"/api/plan-prices/{planId}/room-groups/{roomTypeId}/destinations/{siteId}/children-price",
            TestUtil.ToJsonContent(
                new RoomGroupPriceUpdateChildrenPriceRequest(
                    reqChildrenPrices
                )
            )
        );
    }

    private async Task UpdateSaleSettingOfPlanAsync(
        Plan plan
    )
    {
        var planId = plan.Id;
        var roomTypeId = plan.PlanRoomGroups!.First().RoomGroupId;
        var siteId = plan.PlanSites!.First().SiteId;

        await client.PatchAsync(
            $"/api/plan-prices/{planId}/room-groups/{roomTypeId}/destinations/{siteId}/sale",
            TestUtil.ToJsonContent(
                new RoomGroupPriceUpdateSaleRequest
                {
                    UseAutoExtend = false,
                    AutoExtendMonth = 15,
                    AutoExtendEveryMonthDay = 3
                }
            )
        );
    }

    private async Task UpdatePriceCalendarOfPlanAsync(
        Plan plan
    )
    {
        var planId = plan.Id;
        var roomTypeId = plan.PlanRoomGroups!.First().RoomGroupId;
        var siteId = plan.PlanSites!.First().SiteId;

        await client.PatchAsync(
            $"/api/plan-prices/{planId}/room-groups/{roomTypeId}/destinations/{siteId}/price-calendar",
            TestUtil.ToJsonContent(
                new RoomGroupPriceUpdatePriceCalendarRequest(
                    [
                        new(
                            AppDate.GetId(DateTime.Now.AddDays(0)),
                            1,
                            4,
                            500,
                            false
                        ),
                        new(
                            AppDate.GetId(DateTime.Now.AddDays(1)),
                            1,
                            4,
                            500,
                            false
                        ),
                        new(
                            AppDate.GetId(DateTime.Now.AddDays(2)),
                            1,
                            10,
                            400,
                            false
                        ),
                        new(
                            AppDate.GetId(DateTime.Now.AddDays(3)),
                            1,
                            10,
                            400,
                            false
                        ),
                        new(
                            AppDate.GetId(DateTime.Now.AddDays(4)),
                            1,
                            10,
                            400,
                            false
                        ),
                        new(
                            AppDate.GetId(DateTime.Now.AddDays(5)),
                            1,
                            10,
                            400,
                            false
                        )
                    ]
                )
            )
        );
    }

    private async Task UpdateDiscountOfPlanAsync(
        Plan plan
    )
    {
        var planId = plan.Id;
        var roomTypeId = plan.PlanRoomGroups!.First().RoomGroupId;
        var siteId = plan.PlanSites!.First().SiteId;

        await client.PatchAsync(
            $"/api/plan-prices/{planId}/room-groups/{roomTypeId}/destinations/{siteId}/discount",
            TestUtil.ToJsonContent(
                new RoomGroupPriceUpdateDiscountRequest(
                    [
                        new(
                            1,
                            2,
                            1,
                            10,
                            PriceSettingTypes.Price,
                            50
                        ),
                        new(
                            3,
                            5,
                            1,
                            10,
                            PriceSettingTypes.Price,
                            50
                        ),
                        new(
                            6,
                            9,
                            1,
                            10,
                            PriceSettingTypes.Price,
                            50
                        )
                    ]
                )
            )
        );
    }

    public async Task<(IEnumerable<ReservationEntity> Reservations, List<PersonAgeType> PersonAgeTypes)> CreateReservationsAsync(
        bool isCreate = true
    )
    {
        var dateNow = DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset);
        var personAgeTypes = (await CreatePersonAgeTypesAsync()).ToList();

        var reservationRepo = factory.GetRequiredService<IReservationRepository>()
            ?? throw new ArgumentException(nameof(IReservationRepository));
        var existingReservations = await reservationRepo.GetAllAsync();
        if (existingReservations.Any() && isCreate)
        {
            return (existingReservations, personAgeTypes);
        }

        var plans = (await CreatePlansAsync()).ToList();

        var reservationsToCreate = new List<ReservationEntity>();
        for (var i = 1; i <= 1; i++)
        {
            reservationsToCreate.Add(
                new ReservationEntity
                {
                    Serial = EntityUtil.CreateCode(),
                    FacilityId = facilityInfo.Id,
                    SiteId = plans[i].PlanSites!.First().SiteId,
                    PlanId = plans[i].Id,
                    RoomGroupId = plans[i].PlanRoomGroups!.First().RoomGroupId,
                    CheckInDate = AppDate.GetId(dateNow.AddDays(1)),
                    CheckInTime = new TimeSpan(23, 59, 0),
                    CheckOutTime = dateNow.AddDays(1).TimeOfDay,
                    RestNumber = 1,
                    RoomNumber = 1,
                    Reserver = new()
                    {
                        Name = "Reserver full name",
                        Kana = "Reserver kana",
                        EMail = "test@liberty.com",
                        PostCode = "Reserver post code",
                        Address1 = "Reserver address 1",
                        Address2 = "Reserver address 2",
                        Address3 = "Reserver address 3",
                        Phone = "123456789"
                    },
                    ReservationDateTime = DateTime.UtcNow,
                    ReservationState = ReservationStatus.Confirmed,
                    IsEnabled = true
                }
            );

            await CreateSiteInRoomOfPlanAsync(plans[i]);
            await UpdateStandardPriceOfPlanAsync(plans[i]);
            await UpdateChildrenPriceOfPlanAsync(plans[i], personAgeTypes);
            await UpdateSaleSettingOfPlanAsync(plans[i]);
            await UpdatePriceCalendarOfPlanAsync(plans[i]);
            await UpdateDiscountOfPlanAsync(plans[i]);
        }

        var createdReservations = await reservationRepo.AddRangeAsync(
            reservationsToCreate,
            true
        );

        return (createdReservations, personAgeTypes);
    }

    public async Task<(IEnumerable<ReservationEntity> Reservations, List<PersonAgeType> PersonAgeTypes)>
        CreateReservationsOnlinePaymentAsync()
    {
        var personAgeTypes = (await CreatePersonAgeTypesAsync()).ToList();

        var reservationRepo = factory.GetRequiredService<IReservationRepository>(
            )
            ?? throw new ArgumentException(nameof(IReservationRepository));

        var plans = (await CreatePlansAsync()).ToList();

        var reservationsToCreate = new List<ReservationEntity>();
        for (var i = 1; i <= 1; i++)
        {
            var checkInTime = DateTime.UtcNow.AddHours(i).TimeOfDay;

            reservationsToCreate.Add(
                new ReservationEntity
                {
                    Serial = EntityUtil.CreateCode(),
                    FacilityId = facilityInfo.Id,
                    SiteId = plans[i].PlanSites!.First().SiteId,
                    PlanId = plans[i].Id,
                    RoomGroupId = plans[i].PlanRoomGroups!.First().RoomGroupId,
                    CheckInDate = AppDate.GetId(DateTime.Now.AddDays(1)),
                    CheckInTime = checkInTime,
                    CheckOutTime = new TimeSpan(12, 0, 0),
                    RestNumber = 1,
                    RoomNumber = 1,
                    Reserver = new()
                    {
                        Name = "Reserver full name",
                        Kana = "Reserver kana",
                        EMail = "test@liberty.com",
                        PostCode = "Reserver post code",
                        Address1 = "Reserver address 1",
                        Address2 = "Reserver address 2",
                        Address3 = "Reserver address 3",
                        Phone = "123456789"
                    },
                    ReservationDateTime = DateTime.UtcNow,
                    ReservationState = ReservationStatus.Confirmed,
                    IsEnabled = true,
                    PaymentType = PaymentTypes.OnLinePayment
                }
            );

            await CreateSiteInRoomOfPlanAsync(plans[i]);
            await UpdateStandardPriceOfPlanAsync(plans[i]);
            await UpdateChildrenPriceOfPlanAsync(plans[i], personAgeTypes);
            await UpdateSaleSettingOfPlanAsync(plans[i]);
            await UpdatePriceCalendarOfPlanAsync(plans[i]);
            await UpdateDiscountOfPlanAsync(plans[i]);
        }

        var createdReservations = await reservationRepo.AddRangeAsync(
            reservationsToCreate,
            true
        );

        return (createdReservations, personAgeTypes);
    }

    public async Task<string> CreateOrderReservationAsync(
        long reservationId
    )
    {
        var dbContext = factory.GetRequiredService<ManagerDataContext>(
            )
            ?? throw new ArgumentException(nameof(ManagerDataContext));

        var unixTimestamp = (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
        var orderId = $"order{unixTimestamp}";
        var orderReservationsToCreate = new OrderReservation
        {
            ReservationId = reservationId,
            Order = new Order
            {
                Code = EntityUtil.CreateCode(),
                ApiIssueCode = orderId,
                OrderDateTime = DateTime.UtcNow
            }
        };

        await dbContext.OrderReservations.AddAsync(orderReservationsToCreate);
        await dbContext.SaveChangesAsync();
        return orderId;
    }

    private async Task<long> CreateCancellationCancellationDataAsync(
        long cancellationId
    )
    {
        var dbContext = factory.GetRequiredService<ManagerDataContext>(
            )
            ?? throw new ArgumentException(nameof(ManagerDataContext));

        var cancellationCancellationDataToCreate = new CancellationCancellationData
        {
            CancellationId = cancellationId,
            CancellationData = new CancellationData
            {
                Code = EntityUtil.CreateCode(),
                DayStart = 0,
                DayEnd = 10,
                Rate = 1
            }
        };

        await dbContext.CancellationCancellationDatas.AddAsync(cancellationCancellationDataToCreate);
        await dbContext.SaveChangesAsync();

        return cancellationCancellationDataToCreate.CancellationDataId;
    }

    public async Task<string?> CreatePaymentOnlineAsync(
        string orderId
    )
    {
        var gmoPaymentGatewayService = factory.GetRequiredService<IGmoPaymentGatewayService>();

        var cardNumber = "4111111111111111";
        var cvvCard = "123";
        var amount = 100;
        var tax = 0;
        var currency = "JPY";
        var expiryDate = "2512";

        var paymentEntryTranRequest = new PaymentEntryTranRequest(
            orderId,
            cardNumber,
            cvvCard,
            amount,
            tax,
            currency
        );

        var entryTranResponse = await gmoPaymentGatewayService!.EntryTranAsync(
            paymentEntryTranRequest
        );

        var paymentExecTranRequest = new PaymentExecTranRequest(
            orderId,
            entryTranResponse.AccessId,
            entryTranResponse.AccessPass,
            cardNumber,
            cvvCard,
            expiryDate
        );
        var execTranResponse = await gmoPaymentGatewayService.ExecTranAsync(
            paymentExecTranRequest
        );

        return execTranResponse.TranId;
    }

    private async Task<IEnumerable<PersonAgeType>> CreatePersonAgeTypesAsync()
    {
        var personAgeTypeRepo = factory.GetRequiredService<IFacilityPersonAgeTypeRepository>()
            ?? throw new ArgumentException(nameof(IFacilityPersonAgeTypeRepository));

        var existingPersonAgeTypes = personAgeTypeRepo
            .GetQueryableWithAsNoTracking()
            .Select(x => x.PersonAgeType)
            .ToList();

        if (existingPersonAgeTypes is { Count: > 0 })
        {
            return existingPersonAgeTypes!;
        }

        var personAgeTypesToCreate = new List<FacilityPersonAgeType>
        {
            new()
            {
                FacilityId = facilityInfo.Id,
                PersonAgeType = new()
                {
                    Code = "A",
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "大人" } },
                    AgeMin = 18,
                    AgeMax = null,
                    IsMain = true,
                    IsMaster = true,
                    PersonAgeTypeSpaTaxDatas =
                    [
                        new()
                        {
                            SpaTaxData = new()
                            {
                                PriceMin = 0,
                                PriceMax = 999999,
                                Tax = 200
                            }
                        }
                    ],
                    Meta = new()
                    {
                        PersonAgeGroup = PersonAgeGroups.Adult,
                        FoodBed = FoodBeds.None
                    },
                    IsEnabled = true
                },
                IsEnabled = true
            },
            new()
            {
                FacilityId = facilityInfo.Id,
                PersonAgeType = new()
                {
                    Code = "B",
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "子供" } },
                    AgeMin = 6,
                    AgeMax = 17,
                    IsMain = false,
                    IsMaster = true,
                    PersonAgeTypeSpaTaxDatas =
                    [
                        new()
                        {
                            SpaTaxData = new()
                            {
                                PriceMin = 0,
                                PriceMax = 999999,
                                Tax = 0
                            }
                        }
                    ],
                    Meta = new()
                    {
                        PersonAgeGroup = PersonAgeGroups.Teen,
                        FoodBed = FoodBeds.None
                    },
                    IsEnabled = true
                },
                IsEnabled = true
            },
            new()
            {
                FacilityId = facilityInfo.Id,
                PersonAgeType = new()
                {
                    Code = "C",
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "幼児(食事有・布団有)" } },
                    AgeMin = 0,
                    AgeMax = 5,
                    IsMain = false,
                    IsMaster = true,
                    PersonAgeTypeSpaTaxDatas =
                    [
                        new()
                        {
                            SpaTaxData = new()
                            {
                                PriceMin = 0,
                                PriceMax = 999999,
                                Tax = 0
                            }
                        }
                    ],
                    Meta = new()
                    {
                        PersonAgeGroup = PersonAgeGroups.TeenB,
                        FoodBed = FoodBeds.Bed
                    },
                    IsEnabled = true
                },
                IsEnabled = true
            },
            new()
            {
                FacilityId = facilityInfo.Id,
                PersonAgeType = new()
                {
                    Code = "D",
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "幼児(食事有・布団無)" } },
                    AgeMin = 0,
                    AgeMax = 5,
                    IsMain = false,
                    IsMaster = true,
                    PersonAgeTypeSpaTaxDatas =
                    [
                        new()
                        {
                            SpaTaxData = new()
                            {
                                PriceMin = 0,
                                PriceMax = 999999,
                                Tax = 0
                            }
                        }
                    ],
                    Meta = new()
                    {
                        PersonAgeGroup = PersonAgeGroups.TeenB,
                        FoodBed = FoodBeds.Food
                    },
                    IsEnabled = true
                },
                IsEnabled = true
            },
            new()
            {
                FacilityId = facilityInfo.Id,
                PersonAgeType = new()
                {
                    Code = "E",
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "幼児(食事無・布団有)" } },
                    AgeMin = 0,
                    AgeMax = 5,
                    IsMain = false,
                    IsMaster = true,
                    PersonAgeTypeSpaTaxDatas =
                    [
                        new()
                        {
                            SpaTaxData = new()
                            {
                                PriceMin = 0,
                                PriceMax = 999999,
                                Tax = 0
                            }
                        }
                    ],
                    Meta = new()
                    {
                        PersonAgeGroup = PersonAgeGroups.TeenB,
                        FoodBed = FoodBeds.Bed
                    },
                    IsEnabled = true
                },
                IsEnabled = true
            },
            new()
            {
                FacilityId = facilityInfo.Id,
                PersonAgeType = new()
                {
                    Code = "F",
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "幼児(食事無・布団無)" } },
                    AgeMin = 0,
                    AgeMax = 5,
                    IsMain = false,
                    IsMaster = true,
                    PersonAgeTypeSpaTaxDatas =
                    [
                        new()
                        {
                            SpaTaxData = new()
                            {
                                PriceMin = 0,
                                PriceMax = 999999,
                                Tax = 0
                            }
                        }
                    ],
                    Meta = new()
                    {
                        PersonAgeGroup = PersonAgeGroups.TeenB,
                        FoodBed = FoodBeds.None
                    },
                    IsEnabled = true
                },
                IsEnabled = true
            }
        };

        var createdPersonAgeTypes = await personAgeTypeRepo.AddRangeAsync(
            personAgeTypesToCreate,
            true
        );

        return createdPersonAgeTypes.Select(x => x.PersonAgeType!);
    }

    public async Task<Plan> CreatePlanAsync()
    {
        var planRepo = factory.GetRequiredService<IPlanRepository>();
        var facilityPlanService = factory.GetRequiredService<IFacilityPlanService>();
        var existingPlans = await planRepo!.GetAllAsync();
        if (existingPlans is { Count: > 0 })
        {
            return existingPlans[0];
        }

        var mockPlan = new Plan
        {
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } },
            IsEnabled = true,
            NumberOfStayLimitMin = 1,
            NumberOfStayLimitMax = int.MaxValue,
            IsOnLinePayment = true,
            IsOnSidePayment = true,
            Cancellation = new Cancellation { IsEnabled = true }
        };
        var newPlan = await planRepo.AddAsync(mockPlan, true);

        var mockFacilityPlan = new FacilityPlan
        {
            PlanId = newPlan.Id,
            FacilityId = facilityInfo.Id,
            IsEnabled = true
        };
        _ = await facilityPlanService!.CreateAsync(mockFacilityPlan);

        return newPlan;
    }

    public async Task<RoomGroup> CreateRoomGroupAsync()
    {
        var roomGroupRepo = factory.GetRequiredService<IRoomGroupRepository>();
        var existingRoomGroups = await roomGroupRepo!.GetAllAsync();
        if (existingRoomGroups is { Count: > 0 })
        {
            return existingRoomGroups[0];
        }

        var mockData = new RoomGroup
        {
            Code = EntityUtil.CreateCode(),
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } },
            IsEnabled = true
        };

        var newRoomGroup = await roomGroupRepo.AddAsync(mockData, true);

        return newRoomGroup;
    }

    public async Task<SiteEntity> CreateSiteAsync()
    {
        var siteRepo = factory.GetRequiredService<ISiteRepository>();
        var existingSites = await siteRepo!.GetAllAsync();
        if (existingSites is { Count: > 0 })
        {
            return existingSites[0];
        }

        var mockData = new SiteEntity
        {
            Code = EntityUtil.CreateCode(),
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name}" } },
            Description = "Description",
            Url = "test.com",
            IsEnabled = true
        };

        var newSite = await siteRepo.AddAsync(mockData, true);

        return newSite;
    }

    public async Task<RoomGroupAppDate> CreateRoomGroupAppDateAsync()
    {
        var roomGroup = await CreateRoomGroupAsync();
        var roomGroupAppDateRepo = factory.GetRequiredService<IRoomGroupAppDateRepository>();
        var existingRoomGroups = await roomGroupAppDateRepo!.GetAllAsync();
        if (existingRoomGroups is { Count: > 0 })
        {
            return existingRoomGroups[0];
        }

        var mockRoomGroupAppDate = new RoomGroupAppDate
        {
            RoomGroupId = roomGroup.Id,
            SellNumber = 10,
            AppDate = new AppDate
            {
                DateTime = DateTime.UtcNow.AddDays(1),
                Id = AppDate.GetId(DateTime.UtcNow.AddDays(1))
            },
            IsEnabled = true
        };

        var newRoomGroupAppDate = await roomGroupAppDateRepo!.AddAsync(mockRoomGroupAppDate, true);

        return newRoomGroupAppDate;
    }

    public async Task CreateSystemConfigAsync()
    {
        var systemConfigRepo = factory.GetRequiredService<ISystemConfigRepository>()
            ?? throw new ArgumentException(nameof(ISystemConfigRepository));
        var systemConfig = new SystemConfig
        {
            Code = "test",
            IsEnabled = true,
            CanOnlinePayment = true,
            TemplateFormatData = null
        };
        var systemConfigEntityAdd = await systemConfigRepo.AddAsync(systemConfig, true);
        systemConfigEntityAdd.TemplateFormatData = null;
        await systemConfigRepo.UpdateAsync(systemConfigEntityAdd, true);
    }
}
