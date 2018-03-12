Design:

Doamin
1) Better approach is check all aggreagates. 
2) There is no need to create one repository per Entity.

3) EFDBContext works as UOW, so we do not need to commit every command independetlly

4) The concept about Unity Of Work is not applied. You guys created a UOW per Agregate and is using it to put some repository logic too
The best approach is :

IUnitOfWork ( Commit, RollBack)

public class AuditAppService{

public AuditAppService(IUnitOfWork unityOfWork,IAuditRepository audityRepository)


}

public void Add(Audit audit){

repository.Add(audit);
//example



unityOfWork.Commit();//Is atomic 

}


5) I 




