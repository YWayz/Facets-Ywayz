export class AssignTeamMemberToEventModel {
    teamMemberId: string;
    passCategoryId: string;
    eventId: string;

    initialize(teamMemberId: string, passCategoryId: string, eventId: string) {
        this.eventId = eventId;
        this.passCategoryId = passCategoryId;
        this.teamMemberId = teamMemberId;
    }
}