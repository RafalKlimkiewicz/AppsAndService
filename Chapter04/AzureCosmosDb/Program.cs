//await CreateCosmosResources();
//await CreateProductItems();
//await ListProductItems();
//await ListProductItems("SELECT p.id, p.productName, p.unitPrice FROM Items p WHERE p.category.categoryName = 'Beverages'");
//await DeleteProductItems();
//await CreateInsertProductStoredProcedure();
//await ExecuteInsertProductStoredProcedure();
await ExecuteCalculateTaxUDF(100);
await ListProductItems("SELECT p.id, p.productName, p.unitPrice FROM Items p WHERE p.productId = '78'");
