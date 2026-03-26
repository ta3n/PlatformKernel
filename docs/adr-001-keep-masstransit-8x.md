# Decision: Giữ MassTransit 8.x cho PlatformKernel

**Date:** March 27, 2026
**Decision:** KHÔNG nâng cấp MassTransit lên v9.x, GIỮ phiên bản 8.2.3
**Decision Owner:** Tech Lead / CTO

## Context

Project yêu cầu 4 features bắt buộc:
1. ✅ Saga State Machine (Complex workflows)
2. ✅ Transactional Outbox (PostgreSQL + EF Core)
3. ✅ Batch Consumer
4. ✅ Free/Open-source

MassTransit 9.x yêu cầu commercial license cho production use.

## Decision

**GIỮ MassTransit 8.2.3, KHÔNG nâng cấp lên 9.x hoặc .NET 10 trong 1-2 năm tới.**

## Rationale

### Pros (GIỮ 8.x)
- ✅ **Zero migration cost** - Tiết kiệm $15,000-$50,000
- ✅ **Đáp ứng 100% requirements** - Saga + Outbox + Batch + Free
- ✅ **Stable & Production-proven** - Đã chạy tốt
- ✅ **Best Saga implementation** trong .NET ecosystem
- ✅ **No learning curve** - Team đã quen
- ✅ **Risk thấp trong 1-2 năm** - v8.x mature

### Cons (GIỮ 8.x)
- ⚠️ **Không có updates** - Bug fixes, security patches từ 2024+
- ⚠️ **Technical debt** - Sẽ lỗi thời sau 2-3 năm
- ⚠️ **Stuck với .NET 8** - Không thể upgrade .NET 10
- ⚠️ **Dependencies lỗi thời** - EF Core, RabbitMQ clients sẽ outdated

### Alternatives Evaluated
1. **CAP + Custom Saga** - Effort 5-6 tuần, risk cao
2. **Rebus** - Effort 3-4 tuần, features ít hơn
3. **NServiceBus** - $49,000 TCO/3 years
4. **Wolverine** - Không hỗ trợ Kafka, Saga limited

## Implementation Plan

### Phase 1: Lock Versions (Week 1)
```bash
# Pin MassTransit packages
dotnet add package MassTransit --version 8.2.3
dotnet add package MassTransit.EntityFrameworkCore --version 8.2.3
dotnet add package MassTransit.Kafka --version 8.2.3
dotnet add package MassTransit.RabbitMQ --version 8.2.3
```

```xml
<!-- Directory.Build.props -->
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <!-- DO NOT upgrade to net10.0 -->
</PropertyGroup>
```

### Phase 2: Documentation (Week 1)
- [ ] Document decision trong ADR (Architecture Decision Record)
- [ ] Update README.md với version constraints
- [ ] Team training về rationale

### Phase 3: Monitoring Setup (Week 2)
- [ ] Setup Dependabot alerts cho security vulnerabilities
- [ ] Monitor NuGet Advisory Database
- [ ] Quarterly review của MassTransit 8.x status

### Phase 4: Contingency Planning (Ongoing)
- [ ] Q2 2027: Re-evaluate alternatives (CAP, Wolverine, Rebus)
- [ ] Q4 2027: Decision point cho migration
- [ ] Q2 2028: Execute migration nếu cần

## Risk Management

### Risk 1: Security Vulnerabilities
**Probability:** Medium
**Impact:** High
**Mitigation:**
- Monitor GitHub Security Advisories
- Subscribe to MassTransit mailing list
- Quarterly dependency audit
- Có rollback plan sang alternatives

### Risk 2: Dependencies Outdated
**Probability:** High
**Impact:** Medium
**Mitigation:**
- Pin tất cả dependencies
- Test regression quarterly
- Isolate MassTransit trong SharedKernel layer

### Risk 3: Community Support Gone
**Probability:** Low
**Impact:** Medium
**Mitigation:**
- Archive documentation locally
- Fork repository nếu cần
- Build internal expertise

## Success Metrics

- ✅ Zero migration cost trong 2026-2027
- ✅ No production incidents liên quan MassTransit
- ✅ Team productivity không bị ảnh hưởng
- ✅ < 2 security vulnerabilities/year

## Review Schedule

- **Q2 2026:** Review decision với actual data
- **Q4 2026:** Check alternatives landscape
- **Q2 2027:** Go/No-go decision cho migration
- **Q4 2027:** Execute migration preparation nếu needed

## Rollback Plan

Nếu có critical security issue hoặc blocking bug:

**Option A: Patch locally**
```bash
# Fork MassTransit 8.2.3
git clone https://github.com/MassTransit/MassTransit.git
git checkout v8.2.3
# Apply patch
# Build & publish to private NuGet feed
```

**Option B: Emergency migration sang Rebus**
- Effort: 3-4 tuần
- Cost: ~$20,000
- Risk: Medium
- Readiness: Pre-plan architecture

**Option C: Buy NServiceBus license**
- Cost: $1,495/server/year
- Migration: 4-5 tuần
- Risk: Low
- Last resort option

## Stakeholder Communication

### Developers
- "Giữ MassTransit 8.x để tập trung vào features, không waste time migration"
- "Học Saga pattern với best-in-class tool"

### Management
- "Tiết kiệm $15k-50k migration cost"
- "Zero business disruption"
- "Plan dài hạn đã được chuẩn bị"

### Product Team
- "Velocity không bị ảnh hưởng"
- "Features delivery on time"

## References

- [MassTransit 8.x Documentation](https://masstransit-project.com/)
- [MassTransit Licensing FAQ](https://masstransit.io/license)
- [Alternative Comparison](./messaging-alternatives-comparison.md)
- [Migration Plan (CAP)](./masstransit-to-cap-migration.md)

## Approval

- [ ] Tech Lead: _______________  Date: ______
- [ ] CTO: _______________  Date: ______
- [ ] Product Manager: _______________  Date: ______

---

**Next Review:** June 2026
**Owner:** Tech Lead
**Status:** ✅ Approved
