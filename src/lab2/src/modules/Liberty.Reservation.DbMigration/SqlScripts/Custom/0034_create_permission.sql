INSERT INTO "public"."permission" ("key", "group_name", "parent_code", "item_type", "name", "is_enabled", "is_visible", "record_memo", "is_deleted", "display_order", "created_at", "created_by", "updated_by", "updated_at", "deleted_by", "deleted_at", "code") VALUES ( 'plan_edit', 'plans', NULL, 1, 'プラン編集', 'f', 'f', NULL, 'f', 20250320044737425, 20250320044737425, 'System', 'System', 20250320044737425, NULL, NULL, 'b1f7c20e3a6a4d03a19aec18b2f09258');
INSERT INTO "public"."permission" ("key", "group_name", "parent_code", "item_type", "name", "is_enabled", "is_visible", "record_memo", "is_deleted", "display_order", "created_at", "created_by", "updated_by", "updated_at", "deleted_by", "deleted_at", "code") VALUES ( 'plan.edit.basic', 'plans', 'b1f7c20e3a6a4d03a19aec18b2f09258', 2, '基本設計', 'f', 'f', NULL, 'f', 20250320045259934, 20250320045259934, 'System', 'System', 20250320045259934, NULL, NULL, '2b0347020f354865b527884053056a25');
INSERT INTO "public"."permission" ("key", "group_name", "parent_code", "item_type", "name", "is_enabled", "is_visible", "record_memo", "is_deleted", "display_order", "created_at", "created_by", "updated_by", "updated_at", "deleted_by", "deleted_at", "code") VALUES ( 'plan.edit.basic.nameImport', 'plans', '2b0347020f354865b527884053056a25', 3, 'プラン名(インポート)', 't', 't', NULL, 'f', 20250320070205037, 20250320070205037, 'System', 'System', 20250320070205037, NULL, NULL, 'a3a233aef0034d51bdd81634d2226ffc');

create index ix_permission_parent_order
on public.permission(parent_code, display_order);

create index ix_permission_group_type_order
  on public.permission(group_name, item_type, display_order);



